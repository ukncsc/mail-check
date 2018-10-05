using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.MxSecurityEvaluator.Domain;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dmarc.MxSecurityEvaluator.Dao
{
    public interface ITlsRecordDao
    {
        Task<List<MxRecordTlsProfile>> GetDomainTlsConnectionResults(int domainId);

        Task SaveTlsEvaluatorResults(MxRecordTlsProfile tlsProfile, EvaluatorResults tlsEvaluatorResults);
    }

    public class TlsRecordDao : ITlsRecordDao
    {
        private readonly IConnectionInfoAsync _connectionInfoAsync;
        private readonly ILogger _log;

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public TlsRecordDao(IConnectionInfoAsync connectionInfoAsync, ILogger log)
        {
            _connectionInfoAsync = connectionInfoAsync;
            _log = log;
        }

        public async Task<List<MxRecordTlsProfile>> GetDomainTlsConnectionResults(int domainId)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command =
                    new MySqlCommand(TlsRecordDaoResources.GetDomainTlsConnectionResults, connection);

                command.Parameters.AddWithValue("domain_id", domainId);

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<MxRecordTlsProfile> results = new List<MxRecordTlsProfile>();

                    while (await reader.ReadAsync())
                    {
                        int mxRecordId = reader.GetInt32("mx_record_id");
                        string mxHostname = reader.GetString("hostname");
                        DateTime lastChecked = reader.GetDateTime("last_checked");

                        MxRecordTlsProfile tlsProfile = new MxRecordTlsProfile(
                            mxRecordId,
                            mxHostname,
                            lastChecked,
                            GetTlsConnectionResults(reader));

                        results.Add(tlsProfile);
                    }

                    return results;
                }
            }
        }

        public async Task SaveTlsEvaluatorResults(MxRecordTlsProfile tlsProfile, EvaluatorResults tlsEvaluatorResults)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = BuildTlsEvaluatorResultsCommand(connection, tlsProfile, tlsEvaluatorResults);

                await command.ExecuteNonQueryAsync();
            }
        }

        private MySqlCommand BuildTlsEvaluatorResultsCommand(
            MySqlConnection connection,
            MxRecordTlsProfile tlsProfile,
            EvaluatorResults tlsEvaluatorResults)
        {
            MySqlCommand command = new MySqlCommand(TlsRecordDaoResources.InsertTlsEvaluatorResults, connection);

            command.Parameters.AddWithValue("mx_record_id", tlsProfile.MxRecordId);
            command.Parameters.AddWithValue("last_checked", tlsProfile.LastChecked);
            command.Parameters.AddWithValue("data",
                JsonConvert.SerializeObject(tlsEvaluatorResults, _serializerSettings));

            return command;
        }

        private ConnectionResults GetTlsConnectionResults(DbDataReader reader)
        {
            return string.IsNullOrWhiteSpace(reader.GetString("data"))
                ? null
                : JsonConvert.DeserializeObject<ConnectionResults>(reader.GetString("data"));
        }
    }
}
