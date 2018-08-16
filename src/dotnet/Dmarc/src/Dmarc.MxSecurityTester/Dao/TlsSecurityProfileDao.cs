using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Dao.Entities;
using Dmarc.MxSecurityTester.Util;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dmarc.MxSecurityTester.Dao
{
    internal interface IDomainTlsSecurityProfileDao
    {
        Task<List<DomainTlsSecurityProfile>> GetSecurityProfilesForUpdate();
        Task InsertOrUpdateSecurityProfiles(List<DomainTlsSecurityProfile> domainSecurityProfiles);
    }

    internal class DomainTlsSecurityProfileDao : IDomainTlsSecurityProfileDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly IMxSecurityTesterConfig _mxSecurityTesterConfig;
        private readonly ILogger _log;

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public DomainTlsSecurityProfileDao(IConnectionInfoAsync connectionInfo,
            IMxSecurityTesterConfig mxSecurityTesterConfig,
            ILogger log)
        {
            _connectionInfo = connectionInfo;
            _mxSecurityTesterConfig = mxSecurityTesterConfig;
            _log = log;
        }

        public async Task<List<DomainTlsSecurityProfile>> GetSecurityProfilesForUpdate()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Dictionary<Domain, Dictionary<MxRecord,MxRecordTlsSecurityProfile>> values 
                = new Dictionary<Domain, Dictionary<MxRecord, MxRecordTlsSecurityProfile>>();

            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = new MySqlCommand(TlsSecurityProfileDaoResources.SelectSecurityProfilesToUpdate, connection);

                command.Parameters.AddWithValue("refreshIntervalSeconds", _mxSecurityTesterConfig.RefreshIntervalSeconds);
                command.Parameters.AddWithValue("failureRefreshIntervalSeconds", _mxSecurityTesterConfig.FailureRefreshIntervalSeconds);
                command.Parameters.AddWithValue("limit", _mxSecurityTesterConfig.DomainLimit);

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Domain domain = CreateDomain(reader);
                        Dictionary<MxRecord, MxRecordTlsSecurityProfile> domainValue;
                        if (!values.TryGetValue(domain, out domainValue))
                        {
                            domainValue = new Dictionary<MxRecord, MxRecordTlsSecurityProfile>();
                            values.Add(domain, domainValue);
                        }

                        MxRecord record = CreateMxRecord(reader);
                        MxRecordTlsSecurityProfile profile;
                        if (!domainValue.TryGetValue(record, out profile))
                        {
                            profile = new MxRecordTlsSecurityProfile(record, CreateTlsSecurityProfile(reader));
                            domainValue.Add(record, profile);
                        }

                        X509Certificate2 certificate = CreateCertificate(reader);
                        if (certificate != null)
                        {
                            profile.TlsSecurityProfile.TlsResults.Certificates.Add(certificate);
                        }
                    }
                }
                connection.Close();
            }
            stopwatch.Stop();
            _log.Debug($"Retrieving domains to refresh security profiles for took {stopwatch.Elapsed}");
            return values.Select(_ => new DomainTlsSecurityProfile(_.Key, _.Value.Values.ToList())).ToList();
        }

        private static TlsSecurityProfile CreateTlsSecurityProfile(DbDataReader reader)
        {
            return new TlsSecurityProfile(
                reader.GetUInt64Nullable("tls_security_profile_id"),
                null, CreateTlsTestResults(reader));
        }

        private static TlsTestResults CreateTlsTestResults(DbDataReader reader)
        {
            string jsonData = reader.GetString($"data");

            if (string.IsNullOrEmpty(jsonData))
            {
                return new TlsTestResults(
                    reader.GetInt32("failure_count"), EmptyTlsTestResultsWithoutCertificate,
                    new List<X509Certificate2>());
            }

            TlsTestResultsWithoutCertificate results = CreateTlsTestResult(reader);

            return new TlsTestResults(
                reader.GetInt32("failure_count"), results, null);
        }

        private static TlsTestResultsWithoutCertificate EmptyTlsTestResultsWithoutCertificate => new TlsTestResultsWithoutCertificate(new TlsTestResult(null, null, null, null, null, null, null),
            new TlsTestResult(null, null, null, null, null, null, null),
            new TlsTestResult(null, null, null, null, null, null, null),
            new TlsTestResult(null, null, null, null, null, null, null),
            new TlsTestResult(null, null, null, null, null, null, null),
            new TlsTestResult(null, null, null, null, null, null, null),
            new TlsTestResult(null, null, null, null, null, null, null),
            new TlsTestResult(null, null, null, null, null, null, null),
            new TlsTestResult(null, null, null, null, null, null, null),
            new TlsTestResult(null, null, null, null, null, null, null),
            new TlsTestResult(null, null, null, null, null, null, null),
            new TlsTestResult(null, null, null, null, null, null, null));

        private static TlsTestResultsWithoutCertificate CreateTlsTestResult(DbDataReader reader)
        {
            string jsonData = reader.GetString("data");

            if (string.IsNullOrWhiteSpace(jsonData))
            {
                return EmptyTlsTestResultsWithoutCertificate;
            }

            return JsonConvert.DeserializeObject<TlsTestResultsWithoutCertificate>(jsonData);
        }

        private static X509Certificate2 CreateCertificate(DbDataReader reader)
        {
            ulong? tlsSecurityProfileId = reader.GetUInt64Nullable("tls_security_profile_id");

            return !tlsSecurityProfileId.HasValue || reader.IsDbNull("certificate_thumb_print") || reader.IsDbNull("certificate_raw_data")
                ? null
                : new X509Certificate2(reader.GetByteArray("certificate_raw_data"));
        }

        private static MxRecord CreateMxRecord(DbDataReader reader)
        {
            MxRecord record = new MxRecord(
                reader.GetUInt64("mx_record_id"),
                reader.GetString("mx_record_hostname"));
            return record;
        }

        private static Domain CreateDomain(DbDataReader reader)
        {
            Domain domain = new Domain(reader.GetInt32("domain_id"),
                reader.GetString("domain_name"));
            return domain;
        }

        public async Task InsertOrUpdateSecurityProfiles(List<DomainTlsSecurityProfile> domainSecurityProfiles)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (domainSecurityProfiles.Any())
            {
                using (MySqlConnection connection =
                    new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        List<MxRecordTlsSecurityProfile> profiles =
                            domainSecurityProfiles.SelectMany(_ => _.Profiles).ToList();

                        await InsertOrUpdateSecurityProfiles(profiles, transaction);

                        List<X509Certificate2> certificates = profiles
                            .SelectMany(_ => _.TlsSecurityProfile.TlsResults.Certificates)
                            .GroupBy(_ => _.Thumbprint)
                            .Select(_ => _.First())
                            .ToList();

                        await InsertOrUpdateCertificates(certificates, transaction);

                        List<Tuple<MxRecordTlsSecurityProfile, Tuple<int, X509Certificate2>>> mappings = profiles
                            .Select(_ => Tuple.Create(_, _.TlsSecurityProfile))
                            .Where(_ => _.Item2 != null)
                            .SelectMany(_ =>
                                _.Item2.TlsResults.Certificates.Select((c, i) =>
                                    Tuple.Create(_.Item1, Tuple.Create(i, c))))
                            .ToList();

                        await InsertOrUpdateMappings(mappings, transaction);

                        await transaction.CommitAsync();
                    }

                    connection.Close();
                }
            }

            stopwatch.Stop();
            _log.Debug($"Updating records took {stopwatch.Elapsed}");
        }

        private async Task InsertOrUpdateSecurityProfiles(List<MxRecordTlsSecurityProfile> profiles, MySqlTransaction transaction)
        {
            if (profiles.Any())
            {
                StringBuilder stringBuilder = new StringBuilder(TlsSecurityProfileDaoResources.InsertRecord);
                MySqlCommand command = new MySqlCommand((MySqlConnection)transaction.Connection, transaction);

                for (int i = 0; i < profiles.Count; i++)
                {
                    stringBuilder.AppendFormat(TlsSecurityProfileDaoResources.InsertRecordValueFormatString, i);
                    stringBuilder.Append(i < profiles.Count - 1 ? "," : " ");

                    MxRecordTlsSecurityProfile profile = profiles[i];
                    
                    command.Parameters.AddWithValue($"a{i}", profile.TlsSecurityProfile.Id);
                    command.Parameters.AddWithValue($"b{i}", profile.MxRecord.Id);
                    command.Parameters.AddWithValue($"c{i}", profile.TlsSecurityProfile.EndDate);
                    command.Parameters.AddWithValue($"d{i}", profile.TlsSecurityProfile.TlsResults.FailureCount);
                    command.Parameters.AddWithValue($"e{i}", JsonConvert.SerializeObject(profile.TlsSecurityProfile.TlsResults.Results, _serializerSettings));
                }

                stringBuilder.Append(TlsSecurityProfileDaoResources.InsertRecordOnDuplicateKey);

                command.CommandText = stringBuilder.ToString();

                ulong id = (ulong)await command.ExecuteScalarAsync();
                
                foreach (MxRecordTlsSecurityProfile profile in profiles)
                {
                    if (!profile.TlsSecurityProfile.Id.HasValue)
                    {
                        profile.TlsSecurityProfile.Id = id++;
                    }
                }
            }
        }

        private async Task InsertOrUpdateCertificates(List<X509Certificate2> certificates, MySqlTransaction transaction)
        {
            if (certificates.Any())
            {
                StringBuilder stringBuilder = new StringBuilder(TlsSecurityProfileDaoResources.InsertCertificates);
                MySqlCommand command = new MySqlCommand((MySqlConnection)transaction.Connection, transaction);

                for (int i = 0; i < certificates.Count; i++)
                {
                    stringBuilder.AppendFormat(TlsSecurityProfileDaoResources.InsertCertificateValuesFormatString, i);
                    stringBuilder.Append(i < certificates.Count - 1 ? "," : " ");

                    X509Certificate2 certificate = certificates[i];

                    command.Parameters.AddWithValue($"a{i}", certificate.Thumbprint);
                    command.Parameters.AddWithValue($"b{i}", certificate.RawData);
                }
                

                stringBuilder.Append(TlsSecurityProfileDaoResources.InsertCertificatesOnDuplicateKey);

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertOrUpdateMappings(List<Tuple<MxRecordTlsSecurityProfile, Tuple<int, X509Certificate2>>> mappings, MySqlTransaction transaction)
        {
            if (mappings.Any())
            {
                StringBuilder stringBuilder = new StringBuilder(TlsSecurityProfileDaoResources.InsertMapping);
                MySqlCommand command = new MySqlCommand((MySqlConnection)transaction.Connection, transaction);

                for (int i = 0; i < mappings.Count; i++)
                {
                    stringBuilder.AppendFormat(TlsSecurityProfileDaoResources.InsertMappingValueFormatString, i);
                    stringBuilder.Append(i < mappings.Count - 1 ? "," : " ");

                    Tuple<MxRecordTlsSecurityProfile, Tuple<int, X509Certificate2>> mapping = mappings[i];

                    command.Parameters.AddWithValue($"a{i}", mapping.Item2.Item1);
                    command.Parameters.AddWithValue($"b{i}", mapping.Item1.TlsSecurityProfile.Id);
                    command.Parameters.AddWithValue($"c{i}", mapping.Item2.Item2.Thumbprint);
                }

                stringBuilder.Append(TlsSecurityProfileDaoResources.InsertMappingOnDuplicateKey);

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}