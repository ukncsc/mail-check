using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Dao.Entities;
using MySql.Data.MySqlClient;
using Certificate = Dmarc.MxSecurityTester.Dao.Entities.Certificate;

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

                        Certificate certificate = CreateCertificate(reader);
                        if (certificate != null)
                        {
                            profile.TlsSecurityProfile.Results.Certificates.Add(certificate);
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
                null,
                new TlsTestResults(
                reader.GetInt32("failure_count"),
                CreateTlsTestResult(reader, 1),
                CreateTlsTestResult(reader, 2),
                CreateTlsTestResult(reader, 3),
                CreateTlsTestResult(reader, 4),
                CreateTlsTestResult(reader, 5),
                CreateTlsTestResult(reader, 6),
                CreateTlsTestResult(reader, 7),
                CreateTlsTestResult(reader, 8),
                CreateTlsTestResult(reader, 9),
                CreateTlsTestResult(reader, 10),
                CreateTlsTestResult(reader, 11),
                CreateTlsTestResult(reader, 12),
                null));
        }

        private static TlsTestResult CreateTlsTestResult(DbDataReader reader, int testId)
        {
            return new TlsTestResult(
                (TlsVersion?)reader.GetInt32Nullable($"test{testId}_tls_version"),
                (CipherSuite?)reader.GetInt32Nullable($"test{testId}_cipher_suite"),
                (CurveGroup?)reader.GetInt32Nullable($"test{testId}_curve_group"),
                (SignatureHashAlgorithm?)reader.GetInt32Nullable($"test{testId}_signature_hash_alg"),
                (Error?)reader.GetInt32Nullable($"test{testId}_error")
            );
        }

        private static Certificate CreateCertificate(DbDataReader reader)
        {
            ulong? tlsSecurityProfileId = reader.GetUInt64Nullable("tls_security_profile_id");

            return !tlsSecurityProfileId.HasValue || reader.IsDbNull("certificate_thumb_print")
                ? null
                : new Certificate(
                    reader.GetString("certificate_thumb_print"),
                    reader.GetString("certificate_issuer"),
                    reader.GetString("certificate_subject"),
                    reader.GetDateTime("certificate_start_date"),
                    reader.GetDateTime("certificate_end_date"),
                    reader.GetInt32("certifcate_key_length"),
                    reader.GetString("certificate_algorithm"),
                    reader.GetString("certificate_serial_number"),
                    reader.GetInt32("certificate_version"),
                    reader.GetBoolean("certificate_valid")
                );
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
                using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        List<MxRecordTlsSecurityProfile> profiles = domainSecurityProfiles.SelectMany(_ => _.Profiles).ToList();

                        await InsertOrUpdateSecurityProfiles(profiles, transaction);

                        List<Certificate> certificates = profiles
                            .SelectMany(_ => _.TlsSecurityProfile.Results.Certificates)
                            .GroupBy(_ => _.Thumbprint)
                            .Select(_ => _.First())
                            .ToList();

                        await InsertOrUpdateCertificates(certificates, transaction);

                        List<Tuple<MxRecordTlsSecurityProfile, Tuple<int,Certificate>>> mappings = profiles
                            .Select(_ => Tuple.Create(_, _.TlsSecurityProfile))
                            .Where(_ => _.Item2 != null)
                            .SelectMany(_ => _.Item2.Results.Certificates.Select((c, i) => Tuple.Create(_.Item1, Tuple.Create(i, c))))
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
                    command.Parameters.AddWithValue($"d{i}", profile.TlsSecurityProfile.Results.FailureCount);

                    command.Parameters.AddWithValue($"e{i}", (int?)profile.TlsSecurityProfile.Results.Test1Result.Version);
                    command.Parameters.AddWithValue($"f{i}", (int?)profile.TlsSecurityProfile.Results.Test1Result.CipherSuite);
                    command.Parameters.AddWithValue($"g{i}", (int?)profile.TlsSecurityProfile.Results.Test1Result.CurveGroup);
                    command.Parameters.AddWithValue($"h{i}", (int?)profile.TlsSecurityProfile.Results.Test1Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"i{i}", (int?)profile.TlsSecurityProfile.Results.Test1Result.Error);

                    command.Parameters.AddWithValue($"j{i}", (int?)profile.TlsSecurityProfile.Results.Test2Result.Version);
                    command.Parameters.AddWithValue($"k{i}", (int?)profile.TlsSecurityProfile.Results.Test2Result.CipherSuite);
                    command.Parameters.AddWithValue($"l{i}", (int?)profile.TlsSecurityProfile.Results.Test2Result.CurveGroup);
                    command.Parameters.AddWithValue($"m{i}", (int?)profile.TlsSecurityProfile.Results.Test2Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"n{i}", (int?)profile.TlsSecurityProfile.Results.Test2Result.Error);

                    command.Parameters.AddWithValue($"o{i}", (int?)profile.TlsSecurityProfile.Results.Test3Result.Version);
                    command.Parameters.AddWithValue($"p{i}", (int?)profile.TlsSecurityProfile.Results.Test3Result.CipherSuite);
                    command.Parameters.AddWithValue($"q{i}", (int?)profile.TlsSecurityProfile.Results.Test3Result.CurveGroup);
                    command.Parameters.AddWithValue($"r{i}", (int?)profile.TlsSecurityProfile.Results.Test3Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"s{i}", (int?)profile.TlsSecurityProfile.Results.Test3Result.Error);

                    command.Parameters.AddWithValue($"t{i}", (int?)profile.TlsSecurityProfile.Results.Test4Result.Version);
                    command.Parameters.AddWithValue($"u{i}", (int?)profile.TlsSecurityProfile.Results.Test4Result.CipherSuite);
                    command.Parameters.AddWithValue($"v{i}", (int?)profile.TlsSecurityProfile.Results.Test4Result.CurveGroup);
                    command.Parameters.AddWithValue($"w{i}", (int?)profile.TlsSecurityProfile.Results.Test4Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"x{i}", (int?)profile.TlsSecurityProfile.Results.Test4Result.Error);

                    command.Parameters.AddWithValue($"y{i}", (int?)profile.TlsSecurityProfile.Results.Test5Result.Version);
                    command.Parameters.AddWithValue($"z{i}", (int?)profile.TlsSecurityProfile.Results.Test5Result.CipherSuite);
                    command.Parameters.AddWithValue($"aa{i}", (int?)profile.TlsSecurityProfile.Results.Test5Result.CurveGroup);
                    command.Parameters.AddWithValue($"ab{i}", (int?)profile.TlsSecurityProfile.Results.Test5Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"ac{i}", (int?)profile.TlsSecurityProfile.Results.Test5Result.Error);

                    command.Parameters.AddWithValue($"ad{i}", (int?)profile.TlsSecurityProfile.Results.Test6Result.Version);
                    command.Parameters.AddWithValue($"ae{i}", (int?)profile.TlsSecurityProfile.Results.Test6Result.CipherSuite);
                    command.Parameters.AddWithValue($"af{i}", (int?)profile.TlsSecurityProfile.Results.Test6Result.CurveGroup);
                    command.Parameters.AddWithValue($"ag{i}", (int?)profile.TlsSecurityProfile.Results.Test6Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"ah{i}", (int?)profile.TlsSecurityProfile.Results.Test6Result.Error);

                    command.Parameters.AddWithValue($"ai{i}", (int?)profile.TlsSecurityProfile.Results.Test7Result.Version);
                    command.Parameters.AddWithValue($"aj{i}", (int?)profile.TlsSecurityProfile.Results.Test7Result.CipherSuite);
                    command.Parameters.AddWithValue($"ak{i}", (int?)profile.TlsSecurityProfile.Results.Test7Result.CurveGroup);
                    command.Parameters.AddWithValue($"al{i}", (int?)profile.TlsSecurityProfile.Results.Test7Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"am{i}", (int?)profile.TlsSecurityProfile.Results.Test7Result.Error);

                    command.Parameters.AddWithValue($"an{i}", (int?)profile.TlsSecurityProfile.Results.Test8Result.Version);
                    command.Parameters.AddWithValue($"ao{i}", (int?)profile.TlsSecurityProfile.Results.Test8Result.CipherSuite);
                    command.Parameters.AddWithValue($"ap{i}", (int?)profile.TlsSecurityProfile.Results.Test8Result.CurveGroup);
                    command.Parameters.AddWithValue($"aq{i}", (int?)profile.TlsSecurityProfile.Results.Test8Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"ar{i}", (int?)profile.TlsSecurityProfile.Results.Test8Result.Error);
                                                      
                    command.Parameters.AddWithValue($"as{i}", (int?)profile.TlsSecurityProfile.Results.Test9Result.Version);
                    command.Parameters.AddWithValue($"at{i}", (int?)profile.TlsSecurityProfile.Results.Test9Result.CipherSuite);
                    command.Parameters.AddWithValue($"au{i}", (int?)profile.TlsSecurityProfile.Results.Test9Result.CurveGroup);
                    command.Parameters.AddWithValue($"av{i}", (int?)profile.TlsSecurityProfile.Results.Test9Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"aw{i}", (int?)profile.TlsSecurityProfile.Results.Test9Result.Error);
                                                      
                    command.Parameters.AddWithValue($"ax{i}", (int?)profile.TlsSecurityProfile.Results.Test10Result.Version);
                    command.Parameters.AddWithValue($"ay{i}", (int?)profile.TlsSecurityProfile.Results.Test10Result.CipherSuite);
                    command.Parameters.AddWithValue($"az{i}", (int?)profile.TlsSecurityProfile.Results.Test10Result.CurveGroup);
                    command.Parameters.AddWithValue($"ba{i}", (int?)profile.TlsSecurityProfile.Results.Test10Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"bb{i}", (int?)profile.TlsSecurityProfile.Results.Test10Result.Error);
                                                      
                    command.Parameters.AddWithValue($"bc{i}", (int?)profile.TlsSecurityProfile.Results.Test11Result.Version);
                    command.Parameters.AddWithValue($"bd{i}", (int?)profile.TlsSecurityProfile.Results.Test11Result.CipherSuite);
                    command.Parameters.AddWithValue($"be{i}", (int?)profile.TlsSecurityProfile.Results.Test11Result.CurveGroup);
                    command.Parameters.AddWithValue($"bf{i}", (int?)profile.TlsSecurityProfile.Results.Test11Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"bg{i}", (int?)profile.TlsSecurityProfile.Results.Test11Result.Error);
                                                     
                    command.Parameters.AddWithValue($"bh{i}", (int?)profile.TlsSecurityProfile.Results.Test12Result.Version);
                    command.Parameters.AddWithValue($"bi{i}", (int?)profile.TlsSecurityProfile.Results.Test12Result.CipherSuite);
                    command.Parameters.AddWithValue($"bj{i}", (int?)profile.TlsSecurityProfile.Results.Test12Result.CurveGroup);
                    command.Parameters.AddWithValue($"bk{i}", (int?)profile.TlsSecurityProfile.Results.Test12Result.SignatureHashAlgorithm);
                    command.Parameters.AddWithValue($"bl{i}", (int?)profile.TlsSecurityProfile.Results.Test12Result.Error);
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

        private async Task InsertOrUpdateCertificates(List<Certificate> certificates, MySqlTransaction transaction)
        {
            if (certificates.Any())
            {
                StringBuilder stringBuilder = new StringBuilder(TlsSecurityProfileDaoResources.InsertCertificates);
                MySqlCommand command = new MySqlCommand((MySqlConnection)transaction.Connection, transaction);

                for (int i = 0; i < certificates.Count; i++)
                {
                    stringBuilder.AppendFormat(TlsSecurityProfileDaoResources.InsertCertificateValuesFormatString, i);
                    stringBuilder.Append(i < certificates.Count - 1 ? "," : " ");

                    Certificate certificate = certificates[i];

                    command.Parameters.AddWithValue($"a{i}", certificate.Thumbprint);
                    command.Parameters.AddWithValue($"b{i}", certificate.Issuer);
                    command.Parameters.AddWithValue($"c{i}", certificate.Subject);
                    command.Parameters.AddWithValue($"d{i}", certificate.StartDate);
                    command.Parameters.AddWithValue($"e{i}", certificate.EndDate);
                    command.Parameters.AddWithValue($"f{i}", certificate.KeyLength);
                    command.Parameters.AddWithValue($"g{i}", certificate.Algorithm);
                    command.Parameters.AddWithValue($"h{i}", certificate.SerialNumber);
                    command.Parameters.AddWithValue($"i{i}", certificate.Version);
                    command.Parameters.AddWithValue($"j{i}", certificate.Valid);
                }

                stringBuilder.Append(TlsSecurityProfileDaoResources.InsertCertificatesOnDuplicateKey);

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertOrUpdateMappings(List<Tuple<MxRecordTlsSecurityProfile, Tuple<int, Certificate>>> mappings, MySqlTransaction transaction)
        {
            if (mappings.Any())
            {
                StringBuilder stringBuilder = new StringBuilder(TlsSecurityProfileDaoResources.InsertMapping);
                MySqlCommand command = new MySqlCommand((MySqlConnection)transaction.Connection, transaction);

                for (int i = 0; i < mappings.Count; i++)
                {
                    stringBuilder.AppendFormat(TlsSecurityProfileDaoResources.InsertMappingValueFormatString, i);
                    stringBuilder.Append(i < mappings.Count - 1 ? "," : " ");

                    Tuple<MxRecordTlsSecurityProfile, Tuple<int, Certificate>> mapping = mappings[i];

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