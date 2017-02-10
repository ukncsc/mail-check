using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Dmarc.AggregateReport.Api.Dao.Entities;
using Dmarc.Common.Data;
using MySql.Data.MySqlClient;

namespace Dmarc.AggregateReport.Api.Dao
{
    internal interface IAggregateReportApiDao
    {
        Task<AggregatedStatistics> GetAggregatedHeadlineStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<AggregatedStatistics> GetAggregatedTrustStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<AggregatedStatistics> GetAggregatedComplianceStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<AggregatedStatistics> GetAggregatedDispositionStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<DailyStatistics> GetDailyHeadlineStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<DailyStatistics> GetDailyTrustStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<DailyStatistics> GetDailyComplianceStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<DailyStatistics> GetDailyDispositionStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<bool> DomainExists(int domainId);
        Task<MatchingDomains> GetMatchingDomains(string domainPattern);
    }

    internal class AggregateReportApiDao : IAggregateReportApiDao
    {
        private readonly IConnectionInfo _connectionInfo;

        public AggregateReportApiDao(IConnectionInfo connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task<AggregatedStatistics> GetAggregatedHeadlineStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(AggregateReportApiDaoResources.SelectHeadlineAggregated, connection);

                command.Parameters.AddWithValue("begin_date", beginDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("end_date", endDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                Dictionary<string, int> values = new Dictionary<string, int>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        values.Add("domain_count", reader.GetInt32("domain_count"));
                        values.Add("aggregate_report_count", reader.GetInt32("aggregate_report_count"));
                        values.Add("aggregate_report_record_count", reader.GetInt32("aggregate_report_record_count"));
                        values.Add("total_email_count", (int)reader.GetDecimal("total_email_count"));
                        values.Add("trusted_email_count", (int)reader.GetDecimal("trusted_email_count"));
                        values.Add("untrusted_email_count", (int)reader.GetDecimal("untrusted_email_count"));
                        values.Add("full_compliance_count", (int)reader.GetDecimal("full_compliance_count"));
                        values.Add("dkim_only_count", (int)reader.GetDecimal("dkim_only_count"));
                        values.Add("spf_only_count", (int)reader.GetDecimal("spf_only_count"));
                        values.Add("disposition_none_count", (int)reader.GetDecimal("disposition_none_count"));
                        values.Add("disposition_quarantine_count", (int)reader.GetDecimal("disposition_quarantine_count"));
                        values.Add("disposition_reject_count", (int)reader.GetDecimal("disposition_reject_count"));
                    }
                }
                connection.Close();
                return new AggregatedStatistics(values);
            }
        }

        public async Task<AggregatedStatistics> GetAggregatedTrustStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(AggregateReportApiDaoResources.SelectTrustAggregated, connection);

                command.Parameters.AddWithValue("begin_date", beginDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("end_date", endDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                Dictionary<string, int> values = new Dictionary<string, int>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        values.Add("trusted_email_count", (int)reader.GetDecimal("trusted_email_count"));
                        values.Add("untrusted_email_count", (int)reader.GetDecimal("untrusted_email_count"));
                    }
                }
                connection.Close();
                return new AggregatedStatistics(values);
            }
        }

        public async Task<AggregatedStatistics> GetAggregatedComplianceStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(AggregateReportApiDaoResources.SelectComplianceAggregated, connection);

                command.Parameters.AddWithValue("begin_date", beginDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("end_date", endDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                Dictionary<string, int> values = new Dictionary<string, int>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        values.Add("full_compliance_count", (int)reader.GetDecimal("full_compliance_count"));
                        values.Add("dkim_only_count", (int)reader.GetDecimal("dkim_only_count"));
                        values.Add("spf_only_count", (int)reader.GetDecimal("spf_only_count"));
                    }
                }
                connection.Close();
                return new AggregatedStatistics(values);
            }
        }

        public async Task<AggregatedStatistics> GetAggregatedDispositionStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(AggregateReportApiDaoResources.SelectedDispositionAggregated, connection);

                command.Parameters.AddWithValue("begin_date", beginDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("end_date", endDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                Dictionary<string, int> values = new Dictionary<string, int>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        values.Add("disposition_none_count", (int)reader.GetDecimal("disposition_none_count"));
                        values.Add("disposition_quarantine_count", (int)reader.GetDecimal("disposition_quarantine_count"));
                        values.Add("disposition_reject_count", (int)reader.GetDecimal("disposition_reject_count"));
                    }
                }
                connection.Close();
                return new AggregatedStatistics(values);
            }
        }

        public async Task<DailyStatistics> GetDailyHeadlineStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(AggregateReportApiDaoResources.SelectHeadlineDaily, connection);

                command.Parameters.AddWithValue("begin_date", beginDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("end_date", endDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                Dictionary<DateTime,Dictionary<string, int>> values = new Dictionary<DateTime, Dictionary<string, int>>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    
                    while (await reader.ReadAsync())
                    {
                        DateTime dateTime = reader.GetDateTime("date");
                        Dictionary<string, int> dailyValues = new Dictionary<string, int>
                        {
                            {"domain_count", reader.GetInt32("domain_count")},
                            {"aggregate_report_count", reader.GetInt32("aggregate_report_count")},
                            {"aggregate_report_record_count", reader.GetInt32("aggregate_report_record_count")},
                            {"total_email_count", (int) reader.GetDecimal("total_email_count")},
                            {"trusted_email_count", (int) reader.GetDecimal("trusted_email_count")},
                            {"untrusted_email_count", (int) reader.GetDecimal("untrusted_email_count")},
                            {"full_compliance_count", (int) reader.GetDecimal("full_compliance_count")},
                            {"dkim_only_count", (int) reader.GetDecimal("dkim_only_count")},
                            {"spf_only_count", (int) reader.GetDecimal("spf_only_count")},
                            {"disposition_none_count", (int) reader.GetDecimal("disposition_none_count")},
                            {"disposition_quarantine_count", (int) reader.GetDecimal("disposition_quarantine_count")},
                            {"disposition_reject_count", (int) reader.GetDecimal("disposition_reject_count")}
                        };
                        values.Add(dateTime, dailyValues);
                    }
                }
                connection.Close();
                return new DailyStatistics(values);
            }
        }

        public async Task<DailyStatistics> GetDailyTrustStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(AggregateReportApiDaoResources.SelectTrustDaily, connection);

                command.Parameters.AddWithValue("begin_date", beginDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("end_date", endDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                Dictionary<DateTime, Dictionary<string, int>> values = new Dictionary<DateTime, Dictionary<string, int>>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {

                    while (await reader.ReadAsync())
                    {
                        DateTime dateTime = reader.GetDateTime("date");
                        Dictionary<string, int> dailyValues = new Dictionary<string, int>
                        {
                            {"trusted_email_count", (int) reader.GetDecimal("trusted_email_count")},
                            {"untrusted_email_count", (int) reader.GetDecimal("untrusted_email_count")}
                        };
                        values.Add(dateTime, dailyValues);
                    }
                }
                connection.Close();
                return new DailyStatistics(values);
            }
        }

        public async Task<DailyStatistics> GetDailyComplianceStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(AggregateReportApiDaoResources.SelectComplianceDaily, connection);

                command.Parameters.AddWithValue("begin_date", beginDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("end_date", endDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                Dictionary<DateTime, Dictionary<string, int>> values = new Dictionary<DateTime, Dictionary<string, int>>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {

                    while (await reader.ReadAsync())
                    {
                        DateTime dateTime = reader.GetDateTime("date");
                        Dictionary<string, int> dailyValues = new Dictionary<string, int>
                        {
                            {"full_compliance_count", (int) reader.GetDecimal("full_compliance_count")},
                            {"dkim_only_count", (int) reader.GetDecimal("dkim_only_count")},
                            {"spf_only_count", (int) reader.GetDecimal("spf_only_count")}
                        };
                        values.Add(dateTime, dailyValues);
                    }
                }
                connection.Close();
                return new DailyStatistics(values);
            }
        }

        public async Task<DailyStatistics> GetDailyDispositionStatisticsAsync(DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(AggregateReportApiDaoResources.SelectDispositionDaily, connection);

                command.Parameters.AddWithValue("begin_date", beginDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("end_date", endDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                Dictionary<DateTime, Dictionary<string, int>> values = new Dictionary<DateTime, Dictionary<string, int>>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {

                    while (await reader.ReadAsync())
                    {
                        DateTime dateTime = reader.GetDateTime("date");
                        Dictionary<string, int> dailyValues = new Dictionary<string, int>
                        {
                            {"disposition_none_count", (int) reader.GetDecimal("disposition_none_count")},
                            {"disposition_quarantine_count", (int) reader.GetDecimal("disposition_quarantine_count")},
                            {"disposition_reject_count", (int) reader.GetDecimal("disposition_reject_count")}
                        };
                        values.Add(dateTime, dailyValues);
                    }
                }
                connection.Close();
                return new DailyStatistics(values);
            }
        }

        public async Task<bool> DomainExists(int domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(AggregateReportApiDaoResources.SelectDomain, connection);

                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                object domainResult = await command.ExecuteScalarAsync();

                connection.Close();
                return domainResult != null;
            }
        }

        public async Task<MatchingDomains> GetMatchingDomains(string domainPattern)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(AggregateReportApiDaoResources.SelectMatchingDomains, connection);

                command.Parameters.AddWithValue("domain_pattern", domainPattern);

                command.Prepare();

                List<Domain> domains = new List<Domain>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        domains.Add(new Domain(reader.GetInt32("id"),reader.GetString("domain")));
                    }
                }
                connection.Close();
                return new MatchingDomains(domains);
            }
        }
    }
}