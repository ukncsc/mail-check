using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Domain;
using Dmarc.Common.Data;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Dmarc.AggregateReport.Api.Dao.Sender
{
    public interface ISenderStatisticsDao
    {
        Task<List<SenderStatistics>> GetTrustedSenderStatistics(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<List<SenderStatistics>> GetDkimNoSpfSenderStatistics(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<List<SenderStatistics>> GetSpfNoDkimSenderStatistics(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
        Task<List<SenderStatistics>> GetUntrustedSenderStatistics(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId);
    }

    public class SenderStatisticsDao : ISenderStatisticsDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger _log;

        public SenderStatisticsDao(IConnectionInfoAsync connectionInfo, ILogger<SenderStatisticsDao> log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public Task<List<SenderStatistics>> GetTrustedSenderStatistics(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            return GetSenderStatistics(userId, SenderStatisticsDaoResources.SelectTopTrustedSenders, nameof(GetTrustedSenderStatistics), beginDateUtc, endDateUtc, domainId);
        }

        public Task<List<SenderStatistics>> GetDkimNoSpfSenderStatistics(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            return GetSenderStatistics(userId, SenderStatisticsDaoResources.SelectTopDkimNoSpfSenders, nameof(GetDkimNoSpfSenderStatistics), beginDateUtc, endDateUtc, domainId);
        }

        public Task<List<SenderStatistics>> GetSpfNoDkimSenderStatistics(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            return GetSenderStatistics(userId, SenderStatisticsDaoResources.SelectTopSpfNoDkimSenders, nameof(GetSpfNoDkimSenderStatistics), beginDateUtc, endDateUtc, domainId);
        }

        public Task<List<SenderStatistics>> GetUntrustedSenderStatistics(int userId, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            return GetSenderStatistics(userId, SenderStatisticsDaoResources.SelectTopUntrustedSenders, nameof(GetUntrustedSenderStatistics), beginDateUtc, endDateUtc, domainId);
        }

        private async Task<List<SenderStatistics>> GetSenderStatistics(int userId, string sql, string name, DateTime beginDateUtc, DateTime endDateUtc, int? domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await connection.OpenAsync().ConfigureAwait(false);

                _log.LogDebug($"Connecting to database took: {stopwatch.Elapsed}");
                stopwatch.Restart();

                MySqlCommand command = new MySqlCommand(sql, connection);

                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("begin_date", beginDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("end_date", endDateUtc.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                List<SenderStatistics> senderStatistics = new List<SenderStatistics>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        senderStatistics.Add(new SenderStatistics(
                            reader.GetString("source_ip"), 
                            reader.GetInt32("full_compliance_count"),
                            reader.GetInt32("dkim_only_count"),
                            reader.GetInt32("spf_only_count"),
                            reader.GetInt32("untrusted_email_count")
                        ));
                    }
                }

                _log.LogDebug($"Retrieving data for {name} took: {stopwatch.Elapsed}");
                stopwatch.Stop();

                connection.Close();
                return senderStatistics;
            }
        }
    }
}
