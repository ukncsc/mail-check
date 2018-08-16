using Dmarc.Common.Data;
using Dmarc.Metrics.Api.Domain;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Dmarc.Metrics.Api.Dao
{
    public interface IMetricsDao
    {
        Task<Dictionary<string, MetricsResults>> GetMetrics(DateTime start, DateTime end);
    }

    internal class MetricsDao : IMetricsDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;

        public MetricsDao(IConnectionInfoAsync connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task<Dictionary<string, MetricsResults>> GetMetrics(DateTime start, DateTime end)
        {
            using (var connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (var command = new MySqlCommand(MetricsDaoResources.MetricsQuery, connection))
                {
                    command.Parameters.AddWithValue("start", start.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("end", end.ToString("yyyy-MM-dd"));

                    command.Prepare();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var results = new Dictionary<string, MetricsResults>();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                results.Add(reader.GetDateTime("week_beginning").ToString("yyyy-MM-dd"), GetMetrics(reader));
                            }
                        }

                        return results;
                    }
                }
            }
        }

        private MetricsResults GetMetrics(DbDataReader reader)
        {
            return new MetricsResults()
            {
                DmarcAny = reader.GetInt64("p_any"),
                DmarcMonitor = reader.GetInt64("p_monitor"),
                DmarcActive = reader.GetInt64("p_block"),
                DomainsRegistered = reader.GetInt64("domains"),
                UsersRegistered = reader.GetInt64("users"),
                DomainsAggregateReporting = reader.GetInt64("domains_aggregate_reporting"),
                AggregateReportsReceived = reader.GetInt64("aggregate_report_count"),
                EmailsBlocked = reader.GetInt64("emails_blocked"),
                RuaConfiguredForMailCheck = reader.GetInt64("rua_mc")
            };
        }
    }
}
