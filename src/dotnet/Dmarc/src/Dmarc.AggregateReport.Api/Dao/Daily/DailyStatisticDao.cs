using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Dao.Aggregated;
using Dmarc.AggregateReport.Api.Domain;
using Dmarc.Common.Data;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Dmarc.AggregateReport.Api.Dao.Daily
{
    public interface IDailyStatisticsDao
    {
        Task<DailyStatistics> GetDailyHeadlineStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<DailyStatistics> GetDailyTrustStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<DailyStatistics> GetDailyComplianceStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<DailyStatistics> GetDailyDispositionStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
    }

    internal class DailyStatisticDao : IDailyStatisticsDao
    {
        private readonly IConnectionInfoAsync _connectionInfoAsync;
        private readonly ILogger<AggregatedStatisticsDao> _log;

        public DailyStatisticDao(IConnectionInfoAsync connectionInfoAsync, ILogger<AggregatedStatisticsDao> log)
        {
            _connectionInfoAsync = connectionInfoAsync;
            _log = log;
        }

        public async Task<DailyStatistics> GetDailyHeadlineStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfoAsync.GetConnectionStringAsync()))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await connection.OpenAsync().ConfigureAwait(false);

                _log.LogDebug($"Connecting to database took: {stopwatch.Elapsed}");
                stopwatch.Restart();

                MySqlCommand command = new MySqlCommand(DailyStatisticDaoResources.SelectHeadlineDaily, connection);

                command.Parameters.AddWithValue("userId", userId);
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

                _log.LogDebug($"Retrieving data for { nameof(GetDailyHeadlineStatisticsAsync)} took: {stopwatch.Elapsed}");
                stopwatch.Stop();

                connection.Close();
                return new DailyStatistics(values);
            }
        }

        public async Task<DailyStatistics> GetDailyTrustStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfoAsync.GetConnectionStringAsync()))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await connection.OpenAsync().ConfigureAwait(false);

                _log.LogDebug($"Connecting to database took: {stopwatch.Elapsed}");
                stopwatch.Restart();

                MySqlCommand command = new MySqlCommand(DailyStatisticDaoResources.SelectTrustDaily, connection);

                command.Parameters.AddWithValue("userId", userId);
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

                _log.LogDebug($"Retrieving data for { nameof(GetDailyTrustStatisticsAsync)} took: {stopwatch.Elapsed}");
                stopwatch.Stop();

                connection.Close();
                return new DailyStatistics(values);
            }
        }

        public async Task<DailyStatistics> GetDailyComplianceStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfoAsync.GetConnectionStringAsync()))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await connection.OpenAsync().ConfigureAwait(false);

                _log.LogDebug($"Connecting to database took: {stopwatch.Elapsed}");
                stopwatch.Restart();

                MySqlCommand command = new MySqlCommand(DailyStatisticDaoResources.SelectComplianceDaily, connection);

                command.Parameters.AddWithValue("userId", userId);
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

                _log.LogDebug($"Retrieving data for { nameof(GetDailyComplianceStatisticsAsync)} took: {stopwatch.Elapsed}");
                stopwatch.Stop();

                connection.Close();
                return new DailyStatistics(values);
            }
        }

        public async Task<DailyStatistics> GetDailyDispositionStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfoAsync.GetConnectionStringAsync()))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await connection.OpenAsync().ConfigureAwait(false);

                _log.LogDebug($"Connecting to database took: {stopwatch.Elapsed}");
                stopwatch.Restart();

                MySqlCommand command = new MySqlCommand(DailyStatisticDaoResources.SelectDispositionDaily, connection);

                command.Parameters.AddWithValue("userId", userId);
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

                _log.LogDebug($"Retrieving data for { nameof(GetDailyDispositionStatisticsAsync)} took: {stopwatch.Elapsed}");
                stopwatch.Stop();

                connection.Close();
                return new DailyStatistics(values);
            }
        }
    }
}