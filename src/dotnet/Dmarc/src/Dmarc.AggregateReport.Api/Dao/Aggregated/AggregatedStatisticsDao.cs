using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Domain;
using Dmarc.Common.Data;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Dmarc.AggregateReport.Api.Dao.Aggregated
{
    public interface IAggregatedStatisticsDao
    {
        Task<AggregatedStatistics> GetAggregatedHeadlineStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<AggregatedStatistics> GetAggregatedTrustStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<AggregatedStatistics> GetAggregatedComplianceStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<AggregatedStatistics> GetAggregatedDispositionStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
    }

    internal class AggregatedStatisticsDao : IAggregatedStatisticsDao
    {
        private readonly IConnectionInfoAsync _connectionInfoAsync;
        private readonly ILogger _log;

        public AggregatedStatisticsDao(IConnectionInfoAsync connectionInfoAsync, ILogger<AggregatedStatisticsDao> log)
        {
            _connectionInfoAsync = connectionInfoAsync;
            _log = log;
        }

        public async Task<AggregatedStatistics> GetAggregatedHeadlineStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfoAsync.GetConnectionStringAsync()))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await connection.OpenAsync().ConfigureAwait(false);

                _log.LogDebug($"Connecting to database took: {stopwatch.Elapsed}");
                stopwatch.Restart();

                MySqlCommand command = new MySqlCommand(AggregatedStatisticsDaoResources.SelectHeadlineAggregated, connection);

                command.Parameters.AddWithValue("userId", userId);
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
                        values.Add("untrusted_block_count", (int)reader.GetDecimal("untrusted_block_count"));
                    }
                }

                _log.LogDebug($"Retrieving data for { nameof(GetAggregatedHeadlineStatisticsAsync)} took: {stopwatch.Elapsed}");
                stopwatch.Stop();

                connection.Close();
                return new AggregatedStatistics(values);
            }
        }

        public async Task<AggregatedStatistics> GetAggregatedTrustStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfoAsync.GetConnectionStringAsync()))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await connection.OpenAsync().ConfigureAwait(false);

                _log.LogDebug($"Connecting to database took: {stopwatch.Elapsed}");
                stopwatch.Restart();


                MySqlCommand command = new MySqlCommand(AggregatedStatisticsDaoResources.SelectTrustAggregated, connection);

                command.Parameters.AddWithValue("userId", userId);
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

                _log.LogDebug($"Retrieving data for { nameof(GetAggregatedTrustStatisticsAsync)} took: {stopwatch.Elapsed}");
                stopwatch.Stop();

                connection.Close();
                return new AggregatedStatistics(values);
            }
        }

        public async Task<AggregatedStatistics> GetAggregatedComplianceStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfoAsync.GetConnectionStringAsync()))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await connection.OpenAsync().ConfigureAwait(false);

                _log.LogDebug($"Connecting to database took: {stopwatch.Elapsed}");
                stopwatch.Restart();

                MySqlCommand command = new MySqlCommand(AggregatedStatisticsDaoResources.SelectComplianceAggregated, connection);

                command.Parameters.AddWithValue("userId", userId);
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

                _log.LogDebug($"Retrieving data for { nameof(GetAggregatedComplianceStatisticsAsync)} took: {stopwatch.Elapsed}");
                stopwatch.Stop();

                connection.Close();
                return new AggregatedStatistics(values);
            }
        }

        public async Task<AggregatedStatistics> GetAggregatedDispositionStatisticsAsync(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfoAsync.GetConnectionStringAsync()))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await connection.OpenAsync().ConfigureAwait(false);

                _log.LogDebug($"Connecting to database took: {stopwatch.Elapsed}");
                stopwatch.Restart();

                MySqlCommand command = new MySqlCommand(AggregatedStatisticsDaoResources.SelectedDispositionAggregated, connection);

                command.Parameters.AddWithValue("userId", userId);
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

                _log.LogDebug($"Retrieving data for { nameof(GetAggregatedDispositionStatisticsAsync)} took: {stopwatch.Elapsed}");
                stopwatch.Stop();

                connection.Close();
                return new AggregatedStatistics(values);
            }
        }
    }
}