using Dmarc.Common.Data;
using Dmarc.Common.Interface.Tls.Domain;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Util;
using System;

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

                MySqlCommand command = new MySqlCommand(TlsRecordDaoResources.GetDomainTlsConnectionResults, connection);

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
                            GetTlsConnectionResults(reader, GetCertificates(mxRecordId)));

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

            AddEvaluatorResultToParameters((int)TlsTestType.Tls12AvailableWithBestCipherSuiteSelected, command,
                tlsEvaluatorResults.Tls12AvailableWithBestCipherSuiteSelected);

            AddEvaluatorResultToParameters((int)TlsTestType.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList, command,
                tlsEvaluatorResults.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList);

            AddEvaluatorResultToParameters((int)TlsTestType.Tls12AvailableWithSha2HashFunctionSelected, command,
                tlsEvaluatorResults.Tls12AvailableWithSha2HashFunctionSelected);

            AddEvaluatorResultToParameters((int)TlsTestType.Tls12AvailableWithWeakCipherSuiteNotSelected, command,
                tlsEvaluatorResults.Tls12AvailableWithWeakCipherSuiteNotSelected);

            AddEvaluatorResultToParameters((int)TlsTestType.Tls11AvailableWithBestCipherSuiteSelected, command,
                tlsEvaluatorResults.Tls11AvailableWithBestCipherSuiteSelected);

            AddEvaluatorResultToParameters((int)TlsTestType.Tls11AvailableWithWeakCipherSuiteNotSelected, command,
                tlsEvaluatorResults.Tls11AvailableWithWeakCipherSuiteNotSelected);

            AddEvaluatorResultToParameters((int)TlsTestType.Tls10AvailableWithBestCipherSuiteSelected, command,
                tlsEvaluatorResults.Tls10AvailableWithBestCipherSuiteSelected);

            AddEvaluatorResultToParameters((int)TlsTestType.Tls10AvailableWithWeakCipherSuiteNotSelected, command,
                tlsEvaluatorResults.Tls10AvailableWithWeakCipherSuiteNotSelected);

            AddEvaluatorResultToParameters((int)TlsTestType.Ssl3FailsWithBadCipherSuite, command,
                tlsEvaluatorResults.Ssl3FailsWithBadCipherSuite);

            AddEvaluatorResultToParameters((int)TlsTestType.TlsSecureEllipticCurveSelected, command,
                tlsEvaluatorResults.TlsSecureEllipticCurveSelected);


            AddEvaluatorResultToParameters((int)TlsTestType.TlsSecureDiffieHellmanGroupSelected, command,
                tlsEvaluatorResults.TlsSecureDiffieHellmanGroupSelected);
            
            AddEvaluatorResultToParameters((int)TlsTestType.TlsWeakCipherSuitesRejected, command,
                tlsEvaluatorResults.TlsWeakCipherSuitesRejected);

            return command;
        }

        private void AddEvaluatorResultToParameters(int testNumber, MySqlCommand command, TlsEvaluatorResult evaluatorResult)
        {
            command.Parameters.AddWithValue($"test{testNumber}_result", (int?)evaluatorResult.Result);
            command.Parameters.AddWithValue($"test{testNumber}_description", evaluatorResult.Description);
        }

        private List<X509Certificate2> GetCertificates(int mxRecordId)
        {
            // TODO: fetch certs for given MX ID...
            return new List<X509Certificate2>();
        }

        private ConnectionResults GetTlsConnectionResults(DbDataReader reader, List<X509Certificate2> certificates)
        {
            return new ConnectionResults(
                CreateResult(reader, certificates, (int) TlsTestType.Tls12AvailableWithBestCipherSuiteSelected),
                CreateResult(reader, certificates,
                    (int) TlsTestType.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList),
                CreateResult(reader, certificates, (int) TlsTestType.Tls12AvailableWithSha2HashFunctionSelected),
                CreateResult(reader, certificates, (int) TlsTestType.Tls12AvailableWithWeakCipherSuiteNotSelected),
                CreateResult(reader, certificates, (int) TlsTestType.Tls11AvailableWithBestCipherSuiteSelected),
                CreateResult(reader, certificates, (int) TlsTestType.Tls11AvailableWithWeakCipherSuiteNotSelected),
                CreateResult(reader, certificates, (int) TlsTestType.Tls10AvailableWithBestCipherSuiteSelected),
                CreateResult(reader, certificates, (int) TlsTestType.Tls10AvailableWithWeakCipherSuiteNotSelected),
                CreateResult(reader, certificates, (int) TlsTestType.Ssl3FailsWithBadCipherSuite),
                CreateResult(reader, certificates, (int) TlsTestType.TlsSecureEllipticCurveSelected),
                CreateResult(reader, certificates, (int) TlsTestType.TlsSecureDiffieHellmanGroupSelected),
                CreateResult(reader, certificates, (int) TlsTestType.TlsWeakCipherSuitesRejected));
        }

        private TlsConnectionResult CreateResult(DbDataReader reader, List<X509Certificate2> certificates, int index)
        {
            return new TlsConnectionResult(
                (TlsVersion?)reader.GetInt32Nullable($"test{index}_tls_version"),
                (CipherSuite?)reader.GetInt32Nullable($"test{index}_cipher_suite"),
                (CurveGroup?)reader.GetInt32Nullable($"test{index}_curve_group"),
                (SignatureHashAlgorithm?)reader.GetInt32Nullable($"test{index}_signature_hash_alg"),
                certificates,
                (Error?)reader.GetInt32Nullable($"test{index}_error"));
        }
    }
}
