using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.Common.TestSupport;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Dao;
using Dmarc.MxSecurityTester.Dao.Entities;
using FakeItEasy;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.MxSecurityTester.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class TlsSecurityProfileDaoTests : DatabaseTestBase
    {
        private const string Host1 = "hostname1";
        private const string DomainName1 = "domainName1";
        private const string DomainName2 = "domainName2";
        private const string DomainName3 = "domainName3";
        private const string CertThumbPrint1 = "0AA5B3D767EDEEDA933BD9605FC5B30D5A4384DB";
        private const string CertThumbPrint2 = "89944F8FA8A7D7F0709EF13106411ACA9BC69118";

        private IConnectionInfoAsync _connectionInfo;
        private IMxSecurityTesterConfig _mxSecurityTesterConfig;
        private DomainTlsSecurityProfileDao _domainTlsSecurityProfileDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _connectionInfo = A.Fake<IConnectionInfoAsync>();
            _mxSecurityTesterConfig = A.Fake<IMxSecurityTesterConfig>();
            _domainTlsSecurityProfileDao =
                new DomainTlsSecurityProfileDao(_connectionInfo, _mxSecurityTesterConfig, A.Fake<ILogger>());

            A.CallTo(() => _connectionInfo.GetConnectionStringAsync()).Returns(Task.FromResult(ConnectionString));
        }

        [Test]
        public async Task RetrievesNoSecurityProfilesWhenNoMxRecordsExist()
        {
            SetConfig(_mxSecurityTesterConfig, 10, 1, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();


            Assert.That(tlsSecurityProfiles, Is.Empty);
        }

        [Test]
        public async Task RetrievesEmptySecurityProfilesWhenNoSecurityProfilesExist()
        {
            int successInterval = 10;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(successInterval + 1));

            InsertMxRecord(1, Host1, DomainName1, earlier);

            SetConfig(_mxSecurityTesterConfig, 10, 1, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Certificates, Is.Empty);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Version, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CipherSuite, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CurveGroup, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.SignatureHashAlgorithm, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Error, Is.Null);
        }

        [Test]
        public async Task RetrievesSecurityProfilesToUpdateWhenSuccessfulSecurityProfilesNeedUpdating()
        {
            int successInterval = 10;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(successInterval + 1));

            MxRecord mxRecord = InsertMxRecord(1, Host1, DomainName1, earlier);

            TlsSecurityProfile profile = InsertTlsSecurityProfile(mxRecord.Id, earlier, null, earlier, 0);
            InsertCertificate(1, profile.Id.Value, CertThumbPrint1);
            InsertCertificate(2, profile.Id.Value, CertThumbPrint2);

            SetConfig(_mxSecurityTesterConfig, successInterval, 1, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Certificates, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Version, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CipherSuite, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CurveGroup, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.SignatureHashAlgorithm, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Error, Is.Null);

            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Certificates.Count,
                Is.EqualTo(2));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Certificates[0].Thumbprint,
                Is.EqualTo(CertThumbPrint1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Certificates[1].Thumbprint,
                Is.EqualTo(CertThumbPrint2));
        }

        [Test]
        public async Task RetrievesNoSecurityProfilesWhenSuccessfulSecurityProfilesDontNeedUpdating()
        {
            int successInterval = 10;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(successInterval - 1));

            MxRecord mxRecord = InsertMxRecord(1, Host1, DomainName1, earlier);
            TlsSecurityProfile profile = InsertTlsSecurityProfile(mxRecord.Id, earlier, null, earlier, 0);
            InsertCertificate(1, profile.Id.Value, CertThumbPrint1);

            SetConfig(_mxSecurityTesterConfig, successInterval, 1, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles, Is.Empty);
        }

        [Test]
        public async Task RetrivesSecurityProfilesToUpdateWhenUnsuccessfulSecurityProfilesNeedUpdating()
        {
            int successInterval = 10;
            int failureInterval = 5;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(failureInterval + 1));

            MxRecord mxRecord = InsertMxRecord(1, Host1, DomainName1, earlier);
            InsertTlsSecurityProfile(mxRecord.Id, earlier, null, earlier, 1);

            SetConfig(_mxSecurityTesterConfig, successInterval, failureInterval, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Version, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CipherSuite, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CurveGroup, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.SignatureHashAlgorithm, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Error, Is.Null);
        }

        [Test]
        public async Task
            RetrievesNoSecurityProfilesWhenSuccessfulSecurityPrfilesDontNeedUpdatingAt4FailuresRevertsToSucessInterval()
        {
            int successInterval = 10;
            int failureInterval = 5;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(successInterval - 1));

            MxRecord mxRecord = InsertMxRecord(1, Host1, DomainName1, earlier);
            InsertTlsSecurityProfile(mxRecord.Id, earlier, null, earlier, 4);

            SetConfig(_mxSecurityTesterConfig, successInterval, failureInterval, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task
            RetrievesSecurityProfilesWhenSuccessfulSecurityPrfilesDontNeedUpdatingAt4FailuresRevertsToSucessInterval()
        {
            int successInterval = 10;
            int failureInterval = 5;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(successInterval + 1));

            MxRecord mxRecord = InsertMxRecord(1, Host1, DomainName1, earlier);
            InsertTlsSecurityProfile(mxRecord.Id, earlier, null, earlier, 4);

            SetConfig(_mxSecurityTesterConfig, successInterval, failureInterval, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Version, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CipherSuite, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CurveGroup, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.SignatureHashAlgorithm, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Error, Is.Null);
        }

        [Test]
        public async Task RetrivesNoSecurityProfilesToUpdateWhenUnsuccessfulSecurityProfilesDontNeedUpdating()
        {
            int successInterval = 10;
            int failureInterval = 5;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(failureInterval - 1));

            MxRecord mxRecord = InsertMxRecord(1, Host1, DomainName1, earlier);
            InsertTlsSecurityProfile(mxRecord.Id, earlier, null, earlier, 1);

            SetConfig(_mxSecurityTesterConfig, successInterval, failureInterval, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles, Is.Empty);
        }

        [Test]
        public async Task RetrievesNoSecurityProfilesWhenMxRecordsExpired()
        {
            InsertMxRecord(1, Host1, DomainName1, DateTime.UtcNow, DateTime.UtcNow);

            SetConfig(_mxSecurityTesterConfig, 10, 1, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles, Is.Empty);
        }

        [Test]
        public async Task RetrievesEmptySecurityProfilesIfOnlyExpiredProfilesExist()
        {
            int successInterval = 10;
            int failureInterval = 5;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(failureInterval + 1));

            MxRecord mxRecord = InsertMxRecord(1, Host1, DomainName1, earlier);
            InsertTlsSecurityProfile(mxRecord.Id, earlier, earlier, earlier, 1);

            SetConfig(_mxSecurityTesterConfig, successInterval, failureInterval, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Version, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CipherSuite, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CurveGroup, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.SignatureHashAlgorithm, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Error, Is.Null);
        }

        [Test]
        public async Task RetrievesOnlyNonExpiredSecurityProfiles()
        {
            int successInterval = 10;
            int failureInterval = 5;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(failureInterval + 1));

            MxRecord mxRecord = InsertMxRecord(1, Host1, DomainName1, earlier);

            InsertTlsSecurityProfile(mxRecord.Id, earlier, earlier, earlier, 1);
            InsertTlsSecurityProfile(mxRecord.Id, earlier, null, earlier, 1);

            SetConfig(_mxSecurityTesterConfig, successInterval, failureInterval, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Version, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CipherSuite, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.CurveGroup, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.SignatureHashAlgorithm, Is.Not.Null);
            Assert.That(
                tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected.Error, Is.Null);
        }

        [Test]
        public async Task SecurityProfileLimitIsRespected()
        {
            int successInterval = 10;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(successInterval + 1));

            InsertMxRecord(1, Host1, DomainName1, earlier);
            InsertMxRecord(2, Host1, DomainName2, earlier);
            InsertMxRecord(3, Host1, DomainName3, earlier);

            SetConfig(_mxSecurityTesterConfig, 10, 10, 2);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles =
                await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task InsertsNewRecords()
        {
            MxRecord mxRecord1 = InsertMxRecord(1, Host1, DomainName1, DateTime.Now);

            List<DomainTlsSecurityProfile> domainTlsSecurityProfiles = new List<DomainTlsSecurityProfile>
            {
                new DomainTlsSecurityProfile(new Domain(1, "domain"), new List<MxRecordTlsSecurityProfile>
                {
                    CreateTlsSecurityProfile(mxRecord1, null, CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA,
                        new List<X509Certificate2>
                        {
                            TestCertificates.Certificate1,
                            TestCertificates.Certificate2
                        }),
                    CreateTlsSecurityProfile(mxRecord1, null, CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA,
                        new List<X509Certificate2>
                        {
                            TestCertificates.Certificate1,
                            TestCertificates.Certificate2
                        }),
                })
            };

            await _domainTlsSecurityProfileDao.InsertOrUpdateSecurityProfiles(domainTlsSecurityProfiles);

            List<TlsSecurityProfilePlus> retrievedSecurityProfiles = SelectAllSecurityProfiles();

            Assert.That(domainTlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(retrievedSecurityProfiles.Count));

            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.FailureCount,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.FailureCount));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate,
                Is.EqualTo(retrievedSecurityProfiles[0].EndDate));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.Tls12AvailableWithBestCipherSuiteSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelectedFromReverseList));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithSha2HashFunctionSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.Tls12AvailableWithSha2HashFunctionSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithWeakCipherSuiteNotSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results
                    .Tls12AvailableWithWeakCipherSuiteNotSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls11AvailableWithBestCipherSuiteSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.Tls11AvailableWithBestCipherSuiteSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls11AvailableWithWeakCipherSuiteNotSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results
                    .Tls11AvailableWithWeakCipherSuiteNotSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls10AvailableWithBestCipherSuiteSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.Tls10AvailableWithBestCipherSuiteSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls10AvailableWithWeakCipherSuiteNotSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results
                    .Tls10AvailableWithWeakCipherSuiteNotSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Ssl3FailsWithBadCipherSuite,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.Ssl3FailsWithBadCipherSuite));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .TlsSecureEllipticCurveSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.TlsSecureEllipticCurveSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .TlsSecureDiffieHellmanGroupSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.TlsSecureDiffieHellmanGroupSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .TlsWeakCipherSuitesRejected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.TlsWeakCipherSuitesRejected));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Certificates.Count,
                Is.EqualTo(2));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Certificates[0].Thumbprint,
                Is.EqualTo(CertThumbPrint1));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Certificates[1].Thumbprint,
                Is.EqualTo(CertThumbPrint2));

            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.FailureCount,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.FailureCount));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.EndDate,
                Is.EqualTo(retrievedSecurityProfiles[0].EndDate));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.Tls12AvailableWithBestCipherSuiteSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelectedFromReverseList));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithSha2HashFunctionSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.Tls12AvailableWithSha2HashFunctionSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithWeakCipherSuiteNotSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results
                    .Tls12AvailableWithWeakCipherSuiteNotSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .Tls11AvailableWithBestCipherSuiteSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.Tls11AvailableWithBestCipherSuiteSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .Tls11AvailableWithWeakCipherSuiteNotSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results
                    .Tls11AvailableWithWeakCipherSuiteNotSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .Tls10AvailableWithBestCipherSuiteSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.Tls10AvailableWithBestCipherSuiteSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .Tls10AvailableWithWeakCipherSuiteNotSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results
                    .Tls10AvailableWithWeakCipherSuiteNotSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .Ssl3FailsWithBadCipherSuite,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.Ssl3FailsWithBadCipherSuite));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .TlsSecureEllipticCurveSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.TlsSecureEllipticCurveSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .TlsSecureDiffieHellmanGroupSelected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.TlsSecureDiffieHellmanGroupSelected));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Results
                    .TlsWeakCipherSuitesRejected,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.Results.TlsWeakCipherSuitesRejected));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Certificates.Count,
                Is.EqualTo(2));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Certificates[0].Thumbprint,
                Is.EqualTo(CertThumbPrint1));
            Assert.That(
                domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.Certificates[1].Thumbprint,
                Is.EqualTo(CertThumbPrint2));
        }

        [Test]
        public async Task UpdatesExistingRecords()
        {
            DateTime time = DateTime.UtcNow.AddSeconds(-10);

            MxRecord mxRecord1 = InsertMxRecord(1, Host1, DomainName1, time);
            TlsSecurityProfile securityProfile1 = InsertTlsSecurityProfile(mxRecord1.Id, time, null, time, 0);

            MxRecord mxRecord2 = InsertMxRecord(2, Host1, DomainName2, time);
            TlsSecurityProfile securityProfile2 = InsertTlsSecurityProfile(mxRecord2.Id, time, null, time, 0);

            List<DomainTlsSecurityProfile> domainTlsSecurityProfiles = new List<DomainTlsSecurityProfile>
            {
                new DomainTlsSecurityProfile(new Domain(1, "domain"),
                    new List<MxRecordTlsSecurityProfile>
                    {
                        new MxRecordTlsSecurityProfile(mxRecord1, securityProfile1),
                        new MxRecordTlsSecurityProfile(mxRecord2, securityProfile2)
                    })
            };

            await _domainTlsSecurityProfileDao.InsertOrUpdateSecurityProfiles(domainTlsSecurityProfiles);

            List<TlsSecurityProfilePlus> retrievedSecurityProfiles = SelectAllSecurityProfiles();
            Assert.That(domainTlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(retrievedSecurityProfiles.Count));

            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id,
                Is.EqualTo(retrievedSecurityProfiles[0].Id));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.FailureCount,
                Is.EqualTo(retrievedSecurityProfiles[0].TlsResults.FailureCount));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate,
                Is.EqualTo(retrievedSecurityProfiles[0].EndDate));
            Assert.That(retrievedSecurityProfiles[0].LastChecked, Is.GreaterThan(time));

            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Id,
                Is.EqualTo(retrievedSecurityProfiles[1].Id));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.TlsResults.FailureCount,
                Is.EqualTo(retrievedSecurityProfiles[1].TlsResults.FailureCount));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.EndDate,
                Is.EqualTo(retrievedSecurityProfiles[1].EndDate));
            Assert.That(retrievedSecurityProfiles[1].LastChecked, Is.GreaterThan(time));
        }

        [Test]
        public async Task ExpiresOldRecords()
        {
            DateTime time = DateTime.UtcNow.AddSeconds(-10);

            MxRecord mxRecord1 = InsertMxRecord(1, Host1, DomainName1, time);
            TlsSecurityProfile securityProfile1 = InsertTlsSecurityProfile(mxRecord1.Id, time, null, time, 0);

            MxRecord mxRecord2 = InsertMxRecord(2, Host1, DomainName2, time);
            TlsSecurityProfile securityProfile2 = InsertTlsSecurityProfile(mxRecord2.Id, time, null, time, 0);

            List<DomainTlsSecurityProfile> domainTlsSecurityProfiles = new List<DomainTlsSecurityProfile>
            {
                new DomainTlsSecurityProfile(new Domain(1, "domain"),
                    new List<MxRecordTlsSecurityProfile>
                    {
                        new MxRecordTlsSecurityProfile(mxRecord1, new TlsSecurityProfile(securityProfile1.Id,
                            DateTime.Now, new TlsTestResults(securityProfile1.TlsResults.FailureCount,
                                new TlsTestResultsWithoutCertificate(
                                    securityProfile1.TlsResults.Results.Tls12AvailableWithBestCipherSuiteSelected,
                                    securityProfile1.TlsResults.Results
                                        .Tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
                                    securityProfile1.TlsResults.Results.Tls12AvailableWithSha2HashFunctionSelected,
                                    securityProfile1.TlsResults.Results.Tls12AvailableWithWeakCipherSuiteNotSelected,
                                    securityProfile1.TlsResults.Results.Tls11AvailableWithBestCipherSuiteSelected,
                                    securityProfile1.TlsResults.Results.Tls11AvailableWithWeakCipherSuiteNotSelected,
                                    securityProfile1.TlsResults.Results.Tls10AvailableWithBestCipherSuiteSelected,
                                    securityProfile1.TlsResults.Results.Tls10AvailableWithWeakCipherSuiteNotSelected,
                                    securityProfile1.TlsResults.Results.Ssl3FailsWithBadCipherSuite,
                                    securityProfile1.TlsResults.Results.TlsSecureEllipticCurveSelected,
                                    securityProfile1.TlsResults.Results.TlsSecureDiffieHellmanGroupSelected,
                                    securityProfile1.TlsResults.Results.TlsWeakCipherSuitesRejected),
                                new List<X509Certificate2>()))),

                        new MxRecordTlsSecurityProfile(mxRecord1, new TlsSecurityProfile(securityProfile2.Id,
                            DateTime.Now,
                            new TlsTestResults(securityProfile2.TlsResults.FailureCount,
                                new TlsTestResultsWithoutCertificate(
                                    securityProfile2.TlsResults.Results.Tls12AvailableWithBestCipherSuiteSelected,
                                    securityProfile2.TlsResults.Results
                                        .Tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
                                    securityProfile2.TlsResults.Results.Tls12AvailableWithSha2HashFunctionSelected,
                                    securityProfile2.TlsResults.Results.Tls12AvailableWithWeakCipherSuiteNotSelected,
                                    securityProfile2.TlsResults.Results.Tls11AvailableWithBestCipherSuiteSelected,
                                    securityProfile2.TlsResults.Results.Tls11AvailableWithWeakCipherSuiteNotSelected,
                                    securityProfile2.TlsResults.Results.Tls10AvailableWithBestCipherSuiteSelected,
                                    securityProfile2.TlsResults.Results.Tls10AvailableWithWeakCipherSuiteNotSelected,
                                    securityProfile2.TlsResults.Results.Ssl3FailsWithBadCipherSuite,
                                    securityProfile2.TlsResults.Results.TlsSecureEllipticCurveSelected,
                                    securityProfile2.TlsResults.Results.TlsSecureDiffieHellmanGroupSelected,
                                    securityProfile2.TlsResults.Results.TlsWeakCipherSuitesRejected),
                                new List<X509Certificate2>())))
                    })
            };

            await _domainTlsSecurityProfileDao.InsertOrUpdateSecurityProfiles(domainTlsSecurityProfiles);

            List<TlsSecurityProfilePlus> retrievedSecurityProfiles = SelectAllSecurityProfiles();
            Assert.That(domainTlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(retrievedSecurityProfiles.Count));

            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id,
                Is.EqualTo(retrievedSecurityProfiles[0].Id));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Not.Null);
            Assert.That(retrievedSecurityProfiles[0].LastChecked, Is.GreaterThan(time));

            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Id,
                Is.EqualTo(retrievedSecurityProfiles[1].Id));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.EndDate, Is.Not.Null);
            Assert.That(retrievedSecurityProfiles[1].LastChecked, Is.GreaterThan(time));
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        #region Test Support

        private void SetConfig(IMxSecurityTesterConfig config, int refreshIntervalSeconds,
            int failureRefreshIntervalSeconds, int tlsSecurityProfileLimit)
        {
            A.CallTo(() => config.RefreshIntervalSeconds).Returns(refreshIntervalSeconds);
            A.CallTo(() => config.FailureRefreshIntervalSeconds).Returns(failureRefreshIntervalSeconds);
            A.CallTo(() => config.DomainLimit).Returns(tlsSecurityProfileLimit);
        }

        private Domain InsertDomain(string domainName)
        {

            ulong id = (ulong) MySqlHelper.ExecuteScalar(ConnectionString,
                $@"INSERT INTO `domain`(`name`, `created_by`, `publish`) VALUES('{
                        domainName
                    }', '1', b'1'); SELECT LAST_INSERT_ID();");
            return new Domain((int) id, domainName);
        }

        private MxRecord InsertMxRecord(int preference, string hostname, string domainName, DateTime lastChecked,
            DateTime? endDate = null, int failureCount = 0)
        {
            Domain domain = InsertDomain(domainName);

            string endDateString = endDate == null
                ? "null"
                : $"'{endDate::yyyy-MM-dd HH:mm:ss}'";

            string sql =
                $@"INSERT INTO `dns_record_mx`(`domain_id`, `preference`, `hostname`, `last_checked`, `end_date`, `failure_count`, `result_code`) VALUES({
                        domain.Id
                    }, {preference}, '{hostname}', '{lastChecked:yyyy-MM-dd HH:mm:ss}', {endDateString}, {
                        failureCount
                    }, 0); SELECT LAST_INSERT_ID();";
            ulong id = (ulong) MySqlHelper.ExecuteScalar(ConnectionString, sql);
            return new MxRecord(id, hostname);
        }

        private TlsSecurityProfile InsertTlsSecurityProfile(ulong mxRecordId, DateTime startDate, DateTime? endDate,
            DateTime lastChecked, int failureCount)
        {
            string endDateString = endDate == null
                ? "null"
                : $"'{endDate::yyyy-MM-dd HH:mm:ss}'";

            TlsVersion tlsVersion = TlsVersion.TlsV12;
            CipherSuite cipherSuite = CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA;
            CurveGroup curveGroup = CurveGroup.Ffdhe2048;
            SignatureHashAlgorithm signatureHashAlgorithm = SignatureHashAlgorithm.SHA1_DSA;

            TlsTestResultsWithoutCertificate tlsResult = new TlsTestResultsWithoutCertificate(
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null),
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null),
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null),
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null),
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null),
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null),
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null),
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null),
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null),
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null),
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null),
                new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null, null, null));


            string sql =
                "INSERT INTO `dns_record_mx_tls_profile_results` (`mx_record_id`, `start_date`, `end_date`, `last_checked`, `failure_count`, `data`)" +
                $"VALUES ({mxRecordId}, '{startDate:yyyy-MM-dd HH:mm:ss}', {endDateString}, '{lastChecked:yyyy-MM-dd HH:mm:ss}', {failureCount},  '{JsonConvert.SerializeObject(tlsResult)}'); SELECT LAST_INSERT_ID();";

            ulong id = (ulong) MySqlHelper.ExecuteScalar(ConnectionString, sql);

            TlsTestResult tlsTestResult = new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm,
                null, null, null);
            return new TlsSecurityProfile(id, endDate, new TlsTestResults(failureCount,
                new TlsTestResultsWithoutCertificate(tlsTestResult, tlsTestResult,
                    tlsTestResult, tlsTestResult, tlsTestResult,
                    tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult,
                    tlsTestResult),
                new List<X509Certificate2>()));
        }

        private Certificate InsertCertificate(int sequence, ulong profileId, string thumbPrint)
        {
            string sql = "INSERT INTO `dmarc`.`certificate`(`thumb_print`, `raw_data`)" +
                         $"VALUES('{thumbPrint}','@raw_data');";

            MySqlParameter rawData = new MySqlParameter
            {
                ParameterName = "@raw_data",
                Value = TestCertificates.Certificate1.RawData
            };

            MySqlHelper.ExecuteNonQuery(ConnectionString, sql, rawData);

            InsertCertificateMapping(sequence, profileId, thumbPrint);

            return new Certificate(thumbPrint, TestCertificates.Certificate1.Issuer,
                TestCertificates.Certificate1.Subject, TestCertificates.Certificate1.NotBefore,
                TestCertificates.Certificate1.NotAfter, TestCertificates.Certificate1.GetKeyAlgorithm().Length,
                TestCertificates.Certificate1.GetKeyAlgorithm(), TestCertificates.Certificate1.SerialNumber,
                TestCertificates.Certificate1.Version, true);
        }

        private void InsertCertificateMapping(int sequence, ulong profileId, string thumbPrint)
        {
            string sql =
                "INSERT INTO `certificate_mapping`(`sequence`,`dns_record_mx_tls_profile_id`,`certificate_thumb_print`)" +
                $"VALUES({sequence},{profileId},'{thumbPrint}');";

            MySqlHelper.ExecuteNonQuery(ConnectionString, sql);
        }

        private List<TlsSecurityProfilePlus> SelectAllSecurityProfiles()
        {
            List<TlsSecurityProfilePlus> records = new List<TlsSecurityProfilePlus>();
            using (DbDataReader reader =
                MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM dns_record_mx_tls_profile_results"))
            {
                while (reader.Read())
                {
                    var tlsTestResults = CreateTlsTestResult(reader);

                    TlsSecurityProfilePlus recordEntity = new TlsSecurityProfilePlus(
                        reader.GetUInt64Nullable("id"),
                        null,
                        reader.GetInt32("failure_count"),
                        tlsTestResults.Tls12AvailableWithBestCipherSuiteSelected,
                        tlsTestResults.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
                        tlsTestResults.Tls12AvailableWithSha2HashFunctionSelected,
                        tlsTestResults.Tls12AvailableWithWeakCipherSuiteNotSelected,
                        tlsTestResults.Tls11AvailableWithBestCipherSuiteSelected,
                        tlsTestResults.Tls11AvailableWithWeakCipherSuiteNotSelected,
                        tlsTestResults.Tls10AvailableWithBestCipherSuiteSelected,
                        tlsTestResults.Tls10AvailableWithWeakCipherSuiteNotSelected,
                        tlsTestResults.Ssl3FailsWithBadCipherSuite,
                        tlsTestResults.TlsSecureEllipticCurveSelected,
                        tlsTestResults.TlsSecureDiffieHellmanGroupSelected,
                        tlsTestResults.TlsWeakCipherSuitesRejected,
                        null,
                        reader.GetDateTime("last_checked"),
                        (ulong) reader.GetInt64("mx_record_id"));

                    records.Add(recordEntity);
                }
            }

            foreach (var record in records)
            {
                record.TlsResults.Certificates.AddRange(SelectCertificate(record.Id.Value));
            }

            return records;
        }

        private List<X509Certificate2> SelectCertificate(ulong profileId)
        {
            string sql = "SELECT * " +
                         "FROM certificate_mapping cm " +
                         "JOIN certificate c on c.thumb_print = cm.certificate_thumb_print " +
                         $"WHERE cm.dns_record_mx_tls_profile_id = {profileId} " +
                         "ORDER BY cm.sequence;";

            List<X509Certificate2> certs = new List<X509Certificate2>();

            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, sql))
            {
                while (reader.Read())
                {
                    X509Certificate2 certificate = new X509Certificate2(reader.GetByteArray("raw_data"));

                    certs.Add(certificate);
                }
            }

            return certs;
        }

        private MxRecordTlsSecurityProfile CreateTlsSecurityProfile(MxRecord record, ulong? id = 1,
            CipherSuite cipherSuite = CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA,
            List<X509Certificate2> certificates = null)
        {
            TlsTestResult tlsTestResult = new TlsTestResult(TlsVersion.TlsV12, cipherSuite, CurveGroup.Ffdhe2048,
                SignatureHashAlgorithm.SHA1_DSA, null, null, null);

            return new MxRecordTlsSecurityProfile(record, new TlsSecurityProfile(id, null, new TlsTestResults(0,
                new TlsTestResultsWithoutCertificate(tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult,
                    tlsTestResult, tlsTestResult, tlsTestResult,
                    tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult),
                certificates ?? new List<X509Certificate2>())));
        }

        private static TlsTestResultsWithoutCertificate CreateTlsTestResult(DbDataReader reader)
        {
            string jsonData = reader.GetString("data");

            if (string.IsNullOrWhiteSpace(jsonData))
            {
                return new TlsTestResultsWithoutCertificate(null, null, null, null, null, null, null, null, null, null,
                    null, null);
            }

            return JsonConvert.DeserializeObject<TlsTestResultsWithoutCertificate>(jsonData);
        }


        private class TlsSecurityProfilePlus : TlsSecurityProfile
        {
            public TlsSecurityProfilePlus(ulong? id,
                DateTime? endDate,
                int failureCount,
                TlsTestResult test1Result,
                TlsTestResult test2Result,
                TlsTestResult test3Result,
                TlsTestResult test4Result,
                TlsTestResult test5Result,
                TlsTestResult test6Result,
                TlsTestResult test7Result,
                TlsTestResult test8Result,
                TlsTestResult test9Result,
                TlsTestResult test10Result,
                TlsTestResult test11Result,
                TlsTestResult test12Result,
                List<X509Certificate2> certificates,
                DateTime lastChecked,
                ulong mxRecordId)
                : base(id, endDate, new TlsTestResults(failureCount, new TlsTestResultsWithoutCertificate(test1Result,
                    test2Result, test3Result,
                    test4Result, test5Result, test6Result, test7Result, test8Result,
                    test9Result, test10Result, test11Result, test12Result), certificates))
            {
                LastChecked = lastChecked;
                MxRecordId = mxRecordId;
            }

            public DateTime LastChecked { get; }
            public ulong MxRecordId { get; }
        }

        #endregion Test Support
    }
}
