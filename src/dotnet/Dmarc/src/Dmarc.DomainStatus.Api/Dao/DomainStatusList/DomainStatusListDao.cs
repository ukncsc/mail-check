using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.DomainStatus.Api.Domain;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Dmarc.DomainStatus.Api.Dao.DomainStatusList
{
    public interface IDomainStatusListDao
    {
        Task<long> GetDomainsCount(string search);

        Task<long> GetDomainsCountByUserId(int userId, string search);

        Task<List<DomainSecurityInfo>> GetDomainsSecurityInfo(int page, int pageSize, string search);

        Task<List<DomainSecurityInfo>> GetDomainsSecurityInfoByUserId(int userId, int page, int pageSize, string search);

        Task<List<DomainSecurityInfo>> GetDomainsSecurityInfoByDomainNames(List<string> domainNames);

        Task<WelcomeSearchResult> GetWelcomeSearchResult(string searchTerm);

        Task<List<DomainSecurityInfo>> GetSubdomains(string domain, int page, int pageSize);
    }

    public class DomainStatusListDao : IDomainStatusListDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger<DomainStatusListDao> _log;

        public DomainStatusListDao(IConnectionInfoAsync connectionInfo, ILogger<DomainStatusListDao> log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public Task<long> GetDomainsCount(string search)
        {
            Action<MySqlParameterCollection> addParameters = parameterCollection =>
            {
                parameterCollection.AddWithValue("search", string.IsNullOrWhiteSpace(search) ? null : search);
            };

            return Db.ExecuteScalarTimed<long>(_connectionInfo, DomainStatusListDaoResources.SelectCount,
                addParameters, _ => _log.LogDebug(_), nameof(GetDomainsCount));
        }

        public Task<long> GetDomainsCountByUserId(int userId, string search)
        {
            Action<MySqlParameterCollection> addParameters = parameterCollection =>
            {
                parameterCollection.AddWithValue("userId", userId);
                parameterCollection.AddWithValue("search", string.IsNullOrWhiteSpace(search) ? null : search);
            };

            return Db.ExecuteScalarTimed<long>(_connectionInfo, DomainStatusListDaoResources.SelectCountByUserId,
                addParameters, _ => _log.LogDebug(_), nameof(GetDomainsCountByUserId));
        }

        public Task<List<DomainSecurityInfo>> GetDomainsSecurityInfo(int page, int pageSize, string search)
        {
            Action<MySqlParameterCollection> addParameters = parameterCollection =>
            {
                parameterCollection.AddWithValue("offset", (page - 1) * pageSize);
                parameterCollection.AddWithValue("pageSize", pageSize);
                parameterCollection.AddWithValue("search", search);
            };

            return Db.ExecuteReaderListResultTimed(_connectionInfo, DomainStatusListDaoResources.SelectDomainsSecurityInfo,
                addParameters, CreateDomainSecurityInfo, _ => _log.LogDebug(_), nameof(GetDomainsSecurityInfo));
        }

        public Task<List<DomainSecurityInfo>> GetDomainsSecurityInfoByUserId(int userId, int page, int pageSize, string search)
        {
            Action<MySqlParameterCollection> addParameters = parameterCollection =>
            {
                parameterCollection.AddWithValue("userId", userId);
                parameterCollection.AddWithValue("offset", (page - 1) * pageSize);
                parameterCollection.AddWithValue("pageSize", pageSize);
                parameterCollection.AddWithValue("search", search);
            };

            return Db.ExecuteReaderListResultTimed(_connectionInfo, DomainStatusListDaoResources.SelectDomainsSecurityInfoByUserId,
                addParameters, CreateDomainSecurityInfo, _ => _log.LogDebug(_), nameof(GetDomainsSecurityInfoByUserId));
        }

        public Task<List<DomainSecurityInfo>> GetDomainsSecurityInfoByDomainNames(List<string> domainNames)
        {
            if (!domainNames.Any())
            {
                return Task.FromResult(new List<DomainSecurityInfo>());
            }

            string sql = string.Format(DomainStatusListDaoResources.SelectDomainSecurityInfoByDomainNames,
                    string.Join(",", Enumerable.Range(0, domainNames.Count).Select((_, i) => $"@domainName{i}")));

            Action<MySqlParameterCollection> addParameters = parameterCollection =>
            {
                for (int i = 0; i < domainNames.Count; i++)
                {
                    parameterCollection.AddWithValue($"domainName{i}", domainNames[i]);
                }
            };

            return Db.ExecuteReaderListResultTimed(_connectionInfo, sql,
                addParameters, CreateDomainSecurityInfo, _ => _log.LogDebug(_), nameof(GetDomainsSecurityInfoByDomainNames));
        }

        public Task<WelcomeSearchResult> GetWelcomeSearchResult(string searchTerm) =>
            Db.ExecuteReaderSingleResultTimed(_connectionInfo, DomainStatusListDaoResources.SelectWelcomeSearchResult,
                _ => _.AddWithValue("@term", searchTerm), CreateWelcomeSearchResult, _ => _log.LogDebug(_), nameof(GetWelcomeSearchResult));

        public Task<List<DomainSecurityInfo>> GetSubdomains(string domain, int page, int pageSize)
        {
            Action<MySqlParameterCollection> addParameters = _ =>
            {
                _.AddWithValue("@domain", domain);
                _.AddWithValue("@offset", (page - 1) * pageSize);
                _.AddWithValue("@pageSize", pageSize);
            };

            return Db.ExecuteReaderListResultTimed(_connectionInfo, DomainStatusListDaoResources.SelectSubdomains,
                addParameters, CreateDomainSecurityInfo, _ => _log.LogDebug(_), nameof(GetSubdomains));
        }

        private DomainSecurityInfo CreateDomainSecurityInfo(DbDataReader reader) =>
            new DomainSecurityInfo(
                CreateDomain(reader),
                reader.GetBoolean("has_dmarc"),
                MapStatus((EvaluatorResult)reader.GetInt32("tls_status")), //need to map
                (Status)Enum.Parse(typeof(Status), reader.GetString("dmarc_status"), true),
                (Status)Enum.Parse(typeof(Status), reader.GetString("spf_status"), true));

        private Status MapStatus(EvaluatorResult result)
        {
            switch (result)
            {
                case EvaluatorResult.PASS:
                    return Status.Success;

                case EvaluatorResult.WARNING:
                    return Status.Warning;

                case EvaluatorResult.FAIL:
                    return Status.Error;

                case EvaluatorResult.INCONCLUSIVE:
                    return Status.None;

                case EvaluatorResult.PENDING:
                    return Status.Pending;

                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        private Domain.Domain CreateDomain(DbDataReader reader)
        {
            return new Domain.Domain(
                reader.GetInt32("domain_id"),
                reader.GetString("domain_name"));
        }

        private WelcomeSearchResult CreateWelcomeSearchResult(DbDataReader reader) =>
            new WelcomeSearchResult(reader.GetInt32("id"), reader.GetString("name"));
    }
}
