using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.Common.TestSupport;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Dao;
using Dmarc.MxSecurityTester.Dao.Entities;
using FakeItEasy;
using NUnit.Framework;
using Certificate = Dmarc.MxSecurityTester.Dao.Entities.Certificate;

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
        private const string CertThumbPrint1 = "ABC";
        private const string CertThumbPrint2 = "DEF";

        private IConnectionInfoAsync _connectionInfo;
        private IMxSecurityTesterConfig _mxSecurityTesterConfig;
        private DomainTlsSecurityProfileDao _domainTlsSecurityProfileDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _connectionInfo = A.Fake<IConnectionInfoAsync>();
            _mxSecurityTesterConfig = A.Fake<IMxSecurityTesterConfig>();
            _domainTlsSecurityProfileDao = new DomainTlsSecurityProfileDao(_connectionInfo, _mxSecurityTesterConfig, A.Fake<ILogger>());

            A.CallTo(() => _connectionInfo.GetConnectionStringAsync()).Returns(Task.FromResult(ConnectionString));
        }

        [Test]
        public async Task RetrievesNoSecurityProfilesWhenNoMxRecordsExist()
        {
            SetConfig(_mxSecurityTesterConfig, 10, 1, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles, Is.Empty);
        }

        [Test]
        public async Task RetrievesEmptySecurityProfilesWhenNoSecurityProfilesExist()
        {
            int successInterval = 10;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(successInterval + 1));

            InsertMxRecord(1, Host1, DomainName1, earlier);

            SetConfig(_mxSecurityTesterConfig, 10, 1, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Certificates, Is.Empty);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Version, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CipherSuite, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CurveGroup, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.SignatureHashAlgorithm, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Error, Is.Null);
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

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Certificates, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Version, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CipherSuite, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CurveGroup, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.SignatureHashAlgorithm, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Error, Is.Null);

            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Certificates.Count, Is.EqualTo(2));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Certificates[0].Thumbprint, Is.EqualTo(CertThumbPrint1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Certificates[1].Thumbprint, Is.EqualTo(CertThumbPrint2));
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

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

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

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Version, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CipherSuite, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CurveGroup, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.SignatureHashAlgorithm, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Error, Is.Null);
        }

        [Test]
        public async Task RetrievesNoSecurityProfilesWhenSuccessfulSecurityPrfilesDontNeedUpdatingAt4FailuresRevertsToSucessInterval()
        {
            int successInterval = 10;
            int failureInterval = 5;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(successInterval - 1));

            MxRecord mxRecord = InsertMxRecord(1, Host1, DomainName1, earlier);
            InsertTlsSecurityProfile(mxRecord.Id, earlier, null, earlier, 4);

            SetConfig(_mxSecurityTesterConfig, successInterval, failureInterval, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task RetrievesSecurityProfilesWhenSuccessfulSecurityPrfilesDontNeedUpdatingAt4FailuresRevertsToSucessInterval()
        {
            int successInterval = 10;
            int failureInterval = 5;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(successInterval + 1));

            MxRecord mxRecord = InsertMxRecord(1, Host1, DomainName1, earlier);
            InsertTlsSecurityProfile(mxRecord.Id, earlier, null, earlier, 4);

            SetConfig(_mxSecurityTesterConfig, successInterval, failureInterval, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Version, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CipherSuite, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CurveGroup, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.SignatureHashAlgorithm, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Error, Is.Null);
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

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles, Is.Empty);
        }

        [Test]
        public async Task RetrievesNoSecurityProfilesWhenMxRecordsExpired()
        {
            InsertMxRecord(1, Host1, DomainName1, DateTime.UtcNow, DateTime.UtcNow);

            SetConfig(_mxSecurityTesterConfig, 10, 1, 10);

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

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

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Version, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CipherSuite, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CurveGroup, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.SignatureHashAlgorithm, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Error, Is.Null);
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

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Version, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CipherSuite, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.CurveGroup, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.SignatureHashAlgorithm, Is.Not.Null);
            Assert.That(tlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result.Error, Is.Null);
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

            List<DomainTlsSecurityProfile> tlsSecurityProfiles = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();

            Assert.That(tlsSecurityProfiles.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task InsertsNewRecords()
        {
            MxRecord mxRecord1 = InsertMxRecord(1, Host1, DomainName1, DateTime.Now);

            List<DomainTlsSecurityProfile> domainTlsSecurityProfiles = new List<DomainTlsSecurityProfile> {
                new DomainTlsSecurityProfile(new Domain(1, "domain"), new List<MxRecordTlsSecurityProfile>
                {
                    CreateTlsSecurityProfile(mxRecord1, null, CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA, new List<Certificate>
                        {
                            new Certificate(CertThumbPrint1, "", "", DateTime.Now, DateTime.Now, 2048, "RSA", "123", 1, true),
                            new Certificate(CertThumbPrint2, "", "", DateTime.Now, DateTime.Now, 2048, "RSA", "123", 1, true),
                        }),
                    CreateTlsSecurityProfile(mxRecord1, null, CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA, new List<Certificate>{
                            new Certificate(CertThumbPrint1, "", "", DateTime.Now, DateTime.Now, 2048, "RSA", "123", 1, true),
                            new Certificate(CertThumbPrint2, "", "", DateTime.Now, DateTime.Now, 2048, "RSA", "123", 1, true),
                        }),
                })
            };

            await _domainTlsSecurityProfileDao.InsertOrUpdateSecurityProfiles(domainTlsSecurityProfiles);

            List<TlsSecurityProfilePlus> retrievedSecurityProfiles = SelectAllSecurityProfiles();

            Assert.That(domainTlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(retrievedSecurityProfiles.Count));

            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.FailureCount, Is.EqualTo(retrievedSecurityProfiles[0].Results.FailureCount));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.EqualTo(retrievedSecurityProfiles[0].EndDate));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test1Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test2Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test2Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test3Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test3Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test4Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test4Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test5Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test5Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test6Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test6Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test7Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test7Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test8Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test8Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test9Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test9Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test10Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test10Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test11Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test11Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test12Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test12Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test13Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test13Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Certificates.Count, Is.EqualTo(2));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Certificates[0].Thumbprint, Is.EqualTo(CertThumbPrint1));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Certificates[1].Thumbprint, Is.EqualTo(CertThumbPrint2));

            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.FailureCount, Is.EqualTo(retrievedSecurityProfiles[0].Results.FailureCount));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.EndDate, Is.EqualTo(retrievedSecurityProfiles[0].EndDate));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test1Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test1Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test2Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test2Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test3Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test3Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test4Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test4Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test5Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test5Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test6Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test6Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test7Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test7Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test8Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test8Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test9Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test9Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test10Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test10Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test11Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test11Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test12Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test12Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Test13Result, Is.EqualTo(retrievedSecurityProfiles[0].Results.Test13Result));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Certificates.Count, Is.EqualTo(2));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Certificates[0].Thumbprint, Is.EqualTo(CertThumbPrint1));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.Certificates[1].Thumbprint, Is.EqualTo(CertThumbPrint2));
        }

        [Test]
        public async Task UpdatesExistingRecords()
        {
            DateTime time = DateTime.UtcNow.AddSeconds(-10);

            MxRecord mxRecord1 = InsertMxRecord(1, Host1, DomainName1, time);
            TlsSecurityProfile securityProfile1 = InsertTlsSecurityProfile(mxRecord1.Id, time, null, time, 0);

            MxRecord mxRecord2 = InsertMxRecord(2, Host1, DomainName2, time);
            TlsSecurityProfile securityProfile2 = InsertTlsSecurityProfile(mxRecord2.Id, time, null, time, 0);

            List<DomainTlsSecurityProfile> domainTlsSecurityProfiles = new List<DomainTlsSecurityProfile> {
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

            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.EqualTo(retrievedSecurityProfiles[0].Id));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Results.FailureCount, Is.EqualTo(retrievedSecurityProfiles[0].Results.FailureCount));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.EqualTo(retrievedSecurityProfiles[0].EndDate));
            Assert.That(retrievedSecurityProfiles[0].LastChecked, Is.GreaterThan(time));

            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Id, Is.EqualTo(retrievedSecurityProfiles[1].Id));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Results.FailureCount, Is.EqualTo(retrievedSecurityProfiles[1].Results.FailureCount));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.EndDate, Is.EqualTo(retrievedSecurityProfiles[1].EndDate));
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

            List<DomainTlsSecurityProfile> domainTlsSecurityProfiles = new List<DomainTlsSecurityProfile> {
                new DomainTlsSecurityProfile(new Domain(1, "domain"),
                new List<MxRecordTlsSecurityProfile>
                    {
                        new MxRecordTlsSecurityProfile(mxRecord1, new TlsSecurityProfile(securityProfile1.Id, DateTime.Now, new TlsTestResults(securityProfile1.Results.FailureCount, 
                        securityProfile1.Results.Test1Result,
                        securityProfile1.Results.Test2Result,
                        securityProfile1.Results.Test3Result,
                        securityProfile1.Results.Test4Result,
                        securityProfile1.Results.Test5Result,
                        securityProfile1.Results.Test6Result,
                        securityProfile1.Results.Test7Result,
                        securityProfile1.Results.Test8Result,
                        securityProfile1.Results.Test9Result,
                        securityProfile1.Results.Test10Result,
                        securityProfile1.Results.Test11Result,
                        securityProfile1.Results.Test12Result,
                        securityProfile1.Results.Test13Result,
                        new List<Certificate>()))),

                        new MxRecordTlsSecurityProfile(mxRecord1, new TlsSecurityProfile(securityProfile2.Id, DateTime.Now, 
                        new TlsTestResults(securityProfile2.Results.FailureCount,
                        securityProfile2.Results.Test1Result,
                        securityProfile2.Results.Test2Result,
                        securityProfile2.Results.Test3Result,
                        securityProfile2.Results.Test4Result,
                        securityProfile2.Results.Test5Result,
                        securityProfile2.Results.Test6Result,
                        securityProfile2.Results.Test7Result,
                        securityProfile2.Results.Test8Result,
                        securityProfile2.Results.Test9Result,
                        securityProfile2.Results.Test10Result,
                        securityProfile2.Results.Test11Result,
                        securityProfile2.Results.Test12Result,
                        securityProfile2.Results.Test13Result,
                        new List<Certificate>())))
                    })
            };

            await _domainTlsSecurityProfileDao.InsertOrUpdateSecurityProfiles(domainTlsSecurityProfiles);

            List<TlsSecurityProfilePlus> retrievedSecurityProfiles = SelectAllSecurityProfiles();
            Assert.That(domainTlsSecurityProfiles[0].Profiles.Count, Is.EqualTo(retrievedSecurityProfiles.Count));

            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.EqualTo(retrievedSecurityProfiles[0].Id));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Not.Null);
            Assert.That(retrievedSecurityProfiles[0].LastChecked, Is.GreaterThan(time));

            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.Id, Is.EqualTo(retrievedSecurityProfiles[1].Id));
            Assert.That(domainTlsSecurityProfiles[0].Profiles[1].TlsSecurityProfile.EndDate, Is.Not.Null);
            Assert.That(retrievedSecurityProfiles[1].LastChecked, Is.GreaterThan(time));
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        #region Test Support
        private void SetConfig(IMxSecurityTesterConfig config, int refreshIntervalSeconds, int failureRefreshIntervalSeconds, int tlsSecurityProfileLimit)
        {
            A.CallTo(() => config.RefreshIntervalSeconds).Returns(refreshIntervalSeconds);
            A.CallTo(() => config.FailureRefreshIntervalSeconds).Returns(failureRefreshIntervalSeconds);
            A.CallTo(() => config.DomainLimit).Returns(tlsSecurityProfileLimit);
        }

        private Domain InsertDomain(string domainName)
        {
            MySqlHelper.ExecuteNonQuery(ConnectionString, $@"INSERT INTO `domain`(`name`, `created_by`, `publish`) VALUES('{domainName}', 'test', b'1');");
            ulong id = (ulong)MySqlHelper.ExecuteScalar(ConnectionString, "SELECT LAST_INSERT_ID();");
            return new Domain((int)id, domainName);
        }

        private MxRecord InsertMxRecord(int preference, string hostname, string domainName, DateTime lastChecked, DateTime? endDate = null, int failureCount = 0)
        {
            Domain domain = InsertDomain(domainName);

            string endDateString = endDate == null
                ? "null"
                : $"'{endDate::yyyy-MM-dd HH:mm:ss}'";

            string sql = $@"INSERT INTO `dns_record_mx`(`domain_id`, `preference`, `hostname`, `last_checked`, `end_date`, `failure_count`, `result_code`) VALUES({domain.Id}, {preference}, '{hostname}', '{lastChecked:yyyy-MM-dd HH:mm:ss}', { endDateString }, {failureCount}, 0);";
            MySqlHelper.ExecuteNonQuery(ConnectionString, sql);
            ulong id = (ulong)MySqlHelper.ExecuteScalar(ConnectionString, "SELECT LAST_INSERT_ID();");
            return new MxRecord(id, hostname);
        }

        private TlsSecurityProfile InsertTlsSecurityProfile(ulong mxRecordId, DateTime startDate, DateTime? endDate, DateTime lastChecked, int failureCount)
        {
            string endDateString = endDate == null
                ? "null"
                : $"'{endDate::yyyy-MM-dd HH:mm:ss}'";

            TlsVersion tlsVersion = TlsVersion.TlsV12;
            CipherSuite cipherSuite = CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA;
            CurveGroup curveGroup = CurveGroup.Ffdhe2048;
            SignatureHashAlgorithm signatureHashAlgorithm = SignatureHashAlgorithm.SHA1_DSA;

            string sql = "INSERT INTO `dns_record_mx_tls_profile_2` (`mx_record_id`, `start_date`, `end_date`, `last_checked`, `failure_count`, " +
                         "`test1_tls_version`, `test1_cipher_suite`, `test1_curve_group`, `test1_signature_hash_alg`, `test1_error`, " +
                         "`test2_tls_version`, `test2_cipher_suite`, `test2_curve_group`, `test2_signature_hash_alg`, `test2_error`, " +
                         "`test3_tls_version`, `test3_cipher_suite`, `test3_curve_group`, `test3_signature_hash_alg`, `test3_error`, " +
                         "`test4_tls_version`, `test4_cipher_suite`, `test4_curve_group`, `test4_signature_hash_alg`, `test4_error`, " +
                         "`test5_tls_version`, `test5_cipher_suite`, `test5_curve_group`, `test5_signature_hash_alg`, `test5_error`, " +
                         "`test6_tls_version`, `test6_cipher_suite`, `test6_curve_group`, `test6_signature_hash_alg`, `test6_error`, " +
                         "`test7_tls_version`, `test7_cipher_suite`, `test7_curve_group`, `test7_signature_hash_alg`, `test7_error`, " +
                         "`test8_tls_version`, `test8_cipher_suite`, `test8_curve_group`, `test8_signature_hash_alg`, `test8_error`, " +
                         "`test9_tls_version`, `test9_cipher_suite`, `test9_curve_group`, `test9_signature_hash_alg`, `test9_error`, " +
                         "`test10_tls_version`, `test10_cipher_suite`, `test10_curve_group`, `test10_signature_hash_alg`, `test10_error`, " +
                         "`test11_tls_version`, `test11_cipher_suite`, `test11_curve_group`, `test11_signature_hash_alg`, `test11_error`, " +
                         "`test12_tls_version`, `test12_cipher_suite`, `test12_curve_group`, `test12_signature_hash_alg`, `test12_error`, " +
                         "`test13_tls_version`, `test13_cipher_suite`, `test13_curve_group`, `test13_signature_hash_alg`, `test13_error`) " +
                         $"VALUES ({mxRecordId}, '{startDate:yyyy-MM-dd HH:mm:ss}', {endDateString}, '{lastChecked:yyyy-MM-dd HH:mm:ss}', {failureCount}, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL, " +
                         $"{(int)tlsVersion}, {(int)cipherSuite}, {(int)curveGroup}, {(int)signatureHashAlgorithm}, NULL);";

            MySqlHelper.ExecuteNonQuery(ConnectionString, sql);
            ulong id = (ulong)MySqlHelper.ExecuteScalar(ConnectionString, "SELECT LAST_INSERT_ID();");

            TlsTestResult tlsTestResult = new TlsTestResult(tlsVersion, cipherSuite, curveGroup, signatureHashAlgorithm, null);
            return new TlsSecurityProfile(id, endDate, new TlsTestResults(failureCount, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult,
                tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, new List<Certificate>()));
        }

        private Certificate InsertCertificate(int sequence, ulong profileId, string thumbPrint)
        {
            string issuer = string.Empty;
            string subject = string.Empty;
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = DateTime.UtcNow;
            int keyLength = 2048;
            string algorithm = "RSA";
            string serialNumber = string.Empty;
            int version = 1;
            bool valid = true;

            string sql = "INSERT INTO `dmarc`.`certificate`(`thumb_print`, `issuer`, `subject`, `start_date`, " +
                        "`end_date`, `key_length`, `algorithm`, `serial_number`, `version`, `valid`)" +
                        $"VALUES('{ thumbPrint}','{ issuer }','{ subject }','{startDate:yyyy-MM-dd HH:mm:ss}','{ endDate:yyyy-MM-dd HH:mm:ss}',{ keyLength}," +
                        $"'{ algorithm}','{ serialNumber}',{ version }, {valid});";

            MySqlHelper.ExecuteNonQuery(ConnectionString, sql);

            InsertCertificateMapping(sequence, profileId, thumbPrint);

            return new Certificate(thumbPrint, issuer, subject, startDate, endDate, keyLength, algorithm, serialNumber, version, valid);
        }

        private void InsertCertificateMapping(int sequence, ulong profileId, string thumbPrint)
        {
            string sql = "INSERT INTO `certificate_mapping`(`sequence`,`dns_record_mx_tls_profile_2_id`,`certificate_thumb_print`)" +
                         $"VALUES({sequence},{profileId },'{thumbPrint}');";

            MySqlHelper.ExecuteNonQuery(ConnectionString, sql);
        }

        private List<TlsSecurityProfilePlus> SelectAllSecurityProfiles()
        {
            List<TlsSecurityProfilePlus> records = new List<TlsSecurityProfilePlus>();
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM dns_record_mx_tls_profile_2"))
            {
                while (reader.Read())
                {
                    TlsSecurityProfilePlus recordEntity = new TlsSecurityProfilePlus(
                        reader.GetUInt64Nullable("id"),
                        null,
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
                        CreateTlsTestResult(reader, 13),
                        null,
                        reader.GetDateTime("last_checked"),
                        (ulong)reader.GetInt64("mx_record_id"));

                    records.Add(recordEntity);
                }
            }

            foreach (var record in records)
            {
                record.Results.Certificates.AddRange(SelectCertificate(record.Id.Value));
            }

            return records;
        }

        private List<Certificate> SelectCertificate(ulong profileId)
        {
            string sql = "SELECT * " +
                         "FROM certificate_mapping cm " +
                         "JOIN certificate c on c.thumb_print = cm.certificate_thumb_print " +
                         $"WHERE cm.dns_record_mx_tls_profile_2_id = {profileId} " +
                         "ORDER BY cm.sequence;";

            List<Certificate> certs = new List<Certificate>();

            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, sql))
            {
                while (reader.Read())
                {
                    Certificate recordEntity = new Certificate(
                        reader.GetString("thumb_print"),
                        reader.GetString("issuer"),
                        reader.GetString("subject"),
                        reader.GetDateTime("start_date"),
                        reader.GetDateTime("end_date"),
                        reader.GetInt32("key_length"),
                        reader.GetString("algorithm"),
                        reader.GetString("serial_number"),
                        reader.GetInt32("version"),
                        reader.GetBoolean("valid")
                        );

                    certs.Add(recordEntity);
                }
            }
            return certs;
        }

        private MxRecordTlsSecurityProfile CreateTlsSecurityProfile(MxRecord record, ulong? id = 1, CipherSuite cipherSuite = CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA, List<Certificate> certificates = null)
        {
            TlsTestResult tlsTestResult = new TlsTestResult(TlsVersion.TlsV12, cipherSuite, CurveGroup.Ffdhe2048, SignatureHashAlgorithm.SHA1_DSA, null);

            return new MxRecordTlsSecurityProfile(record, new TlsSecurityProfile(id, null, new TlsTestResults(0, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult,
                tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, certificates ?? new List<Certificate>())));
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
                TlsTestResult test13Result, 
                List<Certificate> certificates, 
                DateTime lastChecked, 
                ulong mxRecordId) 
                : base(id, endDate, new TlsTestResults(failureCount, test1Result, test2Result, test3Result, 
                      test4Result, test5Result, test6Result, test7Result, test8Result, 
                      test9Result, test10Result, test11Result, test12Result, test13Result, certificates))
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
