using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Domain;
using Dmarc.Common.Data;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Dmarc.AggregateReport.Api.Dao.Domain
{
    public interface IDomainsDao
    {
        Task<bool> DomainExists(int userId, int domainId);
        Task<MatchingDomains> GetMatchingDomains(int userId, string domainPattern);
    }

    public class DomainsDao : IDomainsDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger<DomainsDao> _log;

        public DomainsDao(IConnectionInfoAsync connectionInfo, 
            ILogger<DomainsDao> log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public async Task<bool> DomainExists(int userId, int domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await connection.OpenAsync().ConfigureAwait(false);

                _log.LogDebug($"Connecting to database took: {stopwatch.Elapsed}");
                stopwatch.Restart();

                MySqlCommand command = new MySqlCommand(DomainsDaoResources.SelectDomain, connection);

                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                object domainResult = await command.ExecuteScalarAsync();

                _log.LogDebug($"Retrieving data for { nameof(DomainExists)} took: {stopwatch.Elapsed}");
                stopwatch.Stop();

                connection.Close();
                return domainResult != null;
            }
        }

        public async Task<MatchingDomains> GetMatchingDomains(int userId, string domainPattern)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await connection.OpenAsync().ConfigureAwait(false);

                _log.LogDebug($"Connecting to database took: {stopwatch.Elapsed}");
                stopwatch.Restart();

                MySqlCommand command = new MySqlCommand(DomainsDaoResources.SelectMatchingDomains, connection);

                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("domain_pattern", domainPattern);

                command.Prepare();

                List<Api.Domain.Domain> domains = new List<Api.Domain.Domain>();
               
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        domains.Add(new Api.Domain.Domain(reader.GetInt32("id"), reader.GetString("name")));
                    }
                }

                _log.LogDebug($"Retrieving data for { nameof(GetMatchingDomains)} took: {stopwatch.Elapsed}");
                stopwatch.Stop();

                connection.Close();
                return new MatchingDomains(domains);
            }
        }
    }
}
