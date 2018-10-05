using Dmarc.Common.Data;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.DomainStatus.Api.Domain;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dmarc.DomainStatus.Api.Dao.DomainStatus
{
    public interface IDomainStatusDao
    {
        Task<Domain.Domain> GetDomain(int id);

        Task<DomainTlsEvaluatorResults> GetDomainTlsEvaluatorResults(int id);

        Task<string> GetSpfReadModel(int id);

        Task<string> GetDmarcReadModel(int id);

        Task<string> GetDmarcReadModel(string domainName);

        Task<int> GetAggregateReportTotalEmailCount(int domainId, DateTime startDate, DateTime endDate, bool includeSubdomains);

        Task<SortedDictionary<DateTime, AggregateSummaryItem>> GetAggregateReportSummary(int domainId, DateTime startDate, DateTime endDate, bool includeSubdomains);

        Task<List<AggregateReportExportItem>> GetAggregateReportExport(int domainId, DateTime startDate, DateTime endDate);
    }

    internal class DomainStatusDao : IDomainStatusDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger<DomainStatusDao> _log;

        public DomainStatusDao(IConnectionInfoAsync connectionInfo, ILogger<DomainStatusDao> log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public Task<Domain.Domain> GetDomain(int id)
        {
            return Db.ExecuteReaderSingleResultTimed(_connectionInfo, DomainStatusDaoResources.SelectDomainById,
                _ => _.AddWithValue("domainId", id), CreateDomain, _ => _log.LogDebug(_), nameof(GetDomain));
        }

        public Task<DomainTlsEvaluatorResults> GetDomainTlsEvaluatorResults(int id)
        {
            Func<DbDataReader, Task<DomainTlsEvaluatorResults>> createTlsResults = _ => CreateDomainTlsEvaluatorResults(id, _);

            return Db.ExecuteReaderTimed(_connectionInfo, DomainStatusDaoResources.SelectTlsEvaluatorResults,
                _ => _.AddWithValue("domainId", id), createTlsResults, _ => _log.LogDebug(_), nameof(GetDomainTlsEvaluatorResults));
        }

        public Task<string> GetSpfReadModel(int id)
        {
            return Db.ExecuteScalarTimed<string>(_connectionInfo, DomainStatusDaoResources.SelectSpfReadModel,
                _ => _.AddWithValue("domainId", id), _ => _log.LogDebug(_), nameof(GetSpfReadModel));
        }

        public Task<string> GetDmarcReadModel(int id)
        {
            return Db.ExecuteScalarTimed<string>(_connectionInfo, DomainStatusDaoResources.SelectDmarcReadModelByDomainId,
                _ => _.AddWithValue("domainId", id), _ => _log.LogDebug(_), nameof(GetDmarcReadModel));
        }

        public Task<string> GetDmarcReadModel(string domainName)
        {
            return Db.ExecuteScalarTimed<string>(_connectionInfo, DomainStatusDaoResources.SelectDmarcReadModelByDomainName,
               _ => _.AddWithValue("domainName", domainName), _ => _log.LogDebug(_), nameof(GetDmarcReadModel));
        }

        public Task<int> GetAggregateReportTotalEmailCount(int domainId, DateTime startDate, DateTime endDate, bool includeSubdomains)
        {
            Action<MySqlParameterCollection> addParameters = parameterCollection =>
            {
                parameterCollection.AddWithValue("domainId", domainId);
                parameterCollection.AddWithValue("startDate", startDate.ToString("yyyy-MM-dd"));
                parameterCollection.AddWithValue("endDate", endDate.ToString("yyyy-MM-dd"));
            };

            string query = includeSubdomains
                ? DomainStatusDaoResources.SelectAggregateReportTotalEmailCountWithSubdomains
                : DomainStatusDaoResources.SelectAggregateReportTotalEmailCount;

            return Db.ExecuteScalarTimed(_connectionInfo, query, addParameters, _ => _ == DBNull.Value ? 0 : (int)(decimal)_,
                _ => _log.LogDebug(_), nameof(GetAggregateReportTotalEmailCount));
        }

        public Task<SortedDictionary<DateTime, AggregateSummaryItem>> GetAggregateReportSummary(int domainId, DateTime startDate, DateTime endDate, bool includeSubdomains)
        {
            Action<MySqlParameterCollection> addParameters = parameterCollection =>
            {
                parameterCollection.AddWithValue("domainId", domainId);
                parameterCollection.AddWithValue("startDate", startDate.ToString("yyyy-MM-dd"));
                parameterCollection.AddWithValue("endDate", endDate.ToString("yyyy-MM-dd"));
            };

            string query = includeSubdomains
                ? DomainStatusDaoResources.SelectAggregateReportSummaryWithSubdomains
                : DomainStatusDaoResources.SelectAggregateReportSummary;

            return Db.ExecuteReaderTimed(_connectionInfo, query, addParameters, CreateAggegateReportSummary, _ => _log.LogDebug(_), nameof(GetAggregateReportSummary));
        }

        public Task<List<AggregateReportExportItem>> GetAggregateReportExport(int domainId, DateTime startDate, DateTime endDate)
        {
            Action<MySqlParameterCollection> addParameters = parameterCollection =>
            {
                parameterCollection.AddWithValue("domainId", domainId);
                parameterCollection.AddWithValue("startDate", startDate.ToString("yyyy-MM-dd"));
                parameterCollection.AddWithValue("endDate", endDate.ToString("yyyy-MM-dd"));
            };

            return Db.ExecuteReaderListResultTimed(_connectionInfo, DomainStatusDaoResources.SelectAggregateExportData, addParameters,
                CreateAggregateReportExportItem, _ => _log.LogDebug(_), nameof(GetAggregateReportExport));
        }

        private Domain.Domain CreateDomain(DbDataReader reader)
        {
            return new Domain.Domain(
                reader.GetInt32("domain_id"),
                reader.GetString("domain_name"));
        }

        private async Task<DomainTlsEvaluatorResults> CreateDomainTlsEvaluatorResults(int id, DbDataReader reader)
        {
            if (!reader.HasRows)
            {
                return new DomainTlsEvaluatorResults(id, pending: true);
            }

            List<MxTlsEvaluatorResults> mxTlsEvaluatorResults = new List<MxTlsEvaluatorResults>();
            string domainHost = null;

            while (await reader.ReadAsync())
            {
                domainHost = domainHost ?? reader.GetString("name");

                var results = GetTlsEvaluatorResults(reader);

                if (!results.All(_ => _.Result == null && _.Description == null))
                {
                    mxTlsEvaluatorResults.Add(
                    new MxTlsEvaluatorResults(
                        reader.GetInt32("mx_record_id"),
                        reader.GetString("hostname"),
                        reader.GetDateTime("last_checked"),
                        results.Where(_ => _.Result == EvaluatorResult.WARNING).Select(_ => _.Description).ToList(),
                        results.Where(_ => _.Result == EvaluatorResult.FAIL).Select(_ => _.Description).ToList(),
                        results.Where(_ => _.Result == EvaluatorResult.INCONCLUSIVE).Select(_ => _.Description).ToList()
                    ));
                }
            }

            return new DomainTlsEvaluatorResults(id, domainHost, mxTlsEvaluatorResults);
        }

        private List<TlsEvaluatorResult> GetTlsEvaluatorResults(DbDataReader reader)
        {
            var results = new List<TlsEvaluatorResult>();

            if (!string.IsNullOrWhiteSpace(reader.GetString("data")))
            {
                TlsResults tlsResults = JsonConvert.DeserializeObject<TlsResults>(reader.GetString("data"));

                results.Add(tlsResults.Tls12AvailableWithBestCipherSuiteSelected);
                results.Add(tlsResults.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList);
                results.Add(tlsResults.Tls12AvailableWithSha2HashFunctionSelected);
                results.Add(tlsResults.Tls12AvailableWithWeakCipherSuiteNotSelected);
                results.Add(tlsResults.Tls11AvailableWithBestCipherSuiteSelected);
                results.Add(tlsResults.Tls11AvailableWithWeakCipherSuiteNotSelected);
                results.Add(tlsResults.Tls10AvailableWithBestCipherSuiteSelected);
                results.Add(tlsResults.Tls10AvailableWithWeakCipherSuiteNotSelected);
                results.Add(tlsResults.Ssl3FailsWithBadCipherSuite);
                results.Add(tlsResults.TlsSecureEllipticCurveSelected);
                results.Add(tlsResults.TlsSecureDiffieHellmanGroupSelected);
                results.Add(tlsResults.TlsWeakCipherSuitesRejected);
            }

            return results;
        }

        private async Task<SortedDictionary<DateTime, AggregateSummaryItem>> CreateAggegateReportSummary(DbDataReader reader)
        {
            SortedDictionary<DateTime, AggregateSummaryItem> results = new SortedDictionary<DateTime, AggregateSummaryItem>();
            while (await reader.ReadAsync())
            {
                results.Add(reader.GetDateTime("effective_date"), CreateAggregateInfoItem(reader));
            }
            return results;
        }

        private AggregateSummaryItem CreateAggregateInfoItem(DbDataReader reader)
        {
            return new AggregateSummaryItem(
                reader.GetInt32("fully_trusted"),
                reader.GetInt32("partially_trusted"),
                reader.GetInt32("untrusted"),
                reader.GetInt32("quarantined"),
                reader.GetInt32("rejected")
                );
        }

        private AggregateReportExportItem CreateAggregateReportExportItem(DbDataReader reader)
        {
            return new AggregateReportExportItem(
                reader.GetString("header_from"),
                reader.GetString("source_ip"),
                reader.GetString("ptr"),
                reader.GetInt32("count"),
                reader.GetString("spf"),
                reader.GetString("dkim"),
                reader.GetString("disposition"),
                reader.GetString("org_name"),
                reader.GetDateTime("effective_date"));
        }
    }
}
