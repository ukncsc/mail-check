using Dmarc.Common.Data;
using Dmarc.Common.Interface.Tls.Domain;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.MxSecurityEvaluator.Domain;

namespace Dmarc.MxSecurityEvaluator.Dao
{
    public interface ITlsRecordDao
    {
        Task<List<MxRecordTlsProfile>> GetDomainTlsConnectionResults(int domainId);

        Task SaveTlsEvaluatorResults(int mxRecordId, List<TlsEvaluatorResult> tlsEvaluatorResults);
    }

    public class TlsRecordDao : ITlsRecordDao
    {
        private readonly IConnectionInfoAsync _connectionInfoAsync;
        private readonly ILogger _log;

        public TlsRecordDao(IConnectionInfoAsync connectionInfoAsync, ILogger log)
        {
            _connectionInfoAsync = connectionInfoAsync;
            _log = log;
        }

        public async Task<List<MxRecordTlsProfile>> GetDomainTlsConnectionResults(int domainId)
        {
            var connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                var command = new MySqlCommand(TlsRecordDaoResources.GetDomainTlsConnectionResults, connection);

                command.Parameters.AddWithValue("domain_id", domainId);

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    var results = new List<MxRecordTlsProfile>();

                    while (await reader.ReadAsync())
                    {
                        var mxRecordId = reader.GetInt32("mx_record_id");
                        var mxHostname = reader.GetString("hostname");

                        var tlsProfile = new MxRecordTlsProfile(mxRecordId, mxHostname, GetTlsConnectionResults(reader, GetCertificates(mxRecordId)));

                        results.Add(tlsProfile);
                    }

                    return results;
                }
            }
        }

        public async Task SaveTlsEvaluatorResults(int mxRecordId, List<TlsEvaluatorResult> tlsEvaluatorResults)
        {
            var connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                var command = BuildTlsEvaluatorResultsCommand(connection, mxRecordId, tlsEvaluatorResults);

                await command.ExecuteNonQueryAsync();
            }
        }

        private MySqlCommand BuildTlsEvaluatorResultsCommand(
            MySqlConnection connection,
            int mxRecordId,
            List<TlsEvaluatorResult> tlsEvaluatorResults)
        {
            var command = new MySqlCommand(TlsRecordDaoResources.InsertTlsEvaluatorResults, connection);

            command.Parameters.AddWithValue("mx_record_id", mxRecordId);

            for (int i = 0; i < tlsEvaluatorResults.Count; i++)
            {
                var testNumber = i + 1;

                command.Parameters.AddWithValue($"test{testNumber}_result", (int?)tlsEvaluatorResults[i].Result);
                command.Parameters.AddWithValue($"test{testNumber}_description", tlsEvaluatorResults[i].Description);
            }

            return command;
        }

        private List<X509Certificate2> GetCertificates(int mxRecordId)
        {
            // TODO: fetch certs for given MX ID...
            return new List<X509Certificate2>();
        }

        private List<TlsConnectionResult> GetTlsConnectionResults(DbDataReader reader, List<X509Certificate2> certificates)
        {
            var tlsConnectionResults = new List<TlsConnectionResult>();

            for (int i = 1; i <= 13; i++)
            {
                tlsConnectionResults.Add(
                    new TlsConnectionResult(
                        (TlsVersion?)reader.GetInt32Nullable($"test{i}_tls_version"),
                        (CipherSuite?)reader.GetInt32Nullable($"test{i}_cipher_suite"),
                        (CurveGroup?)reader.GetInt32Nullable($"test{i}_curve_group"),
                        (SignatureHashAlgorithm?)reader.GetInt32Nullable($"test{i}_signature_hash_alg"),
                        certificates,
                        (Error?)reader.GetInt32Nullable($"test{i}_error")));
            }

            return tlsConnectionResults;
        }
    }
}
