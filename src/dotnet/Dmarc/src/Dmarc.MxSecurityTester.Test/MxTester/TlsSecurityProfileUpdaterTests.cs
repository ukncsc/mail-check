using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.Common.Logging;
using Dmarc.MxSecurityTester.Dao.Entities;
using Dmarc.MxSecurityTester.MxTester;
using FakeItEasy;
using NUnit.Framework;
using Certificate = Dmarc.MxSecurityTester.Dao.Entities.Certificate;

namespace Dmarc.MxSecurityTester.Test.MxTester
{
    [TestFixture]
    public class TlsSecurityProfileUpdaterTests
    {
        private ICachingTlsSecurityTesterAdapator _tlsSecurityTester;
        private TlsSecurityProfileUpdater _tlsSecurityProfileUpdater;

        [SetUp]
        public void SetUp()
        {
            _tlsSecurityTester = A.Fake<ICachingTlsSecurityTesterAdapator>();
            _tlsSecurityProfileUpdater = new TlsSecurityProfileUpdater(_tlsSecurityTester, A.Fake<ILogger>());
        }

        [Test]
        public async Task NoDifferenceInRecordsRecordIsReturned()
        {
            MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile1 = CreateTlsSecurityProfile();

            A.CallTo(() => _tlsSecurityTester.Test(mxRecordTlsSecurityProfile1)).Returns(Task.FromResult(mxRecordTlsSecurityProfile1));

            List<DomainTlsSecurityProfile> securityProfiles = await _tlsSecurityProfileUpdater.UpdateSecurityProfiles(
                new List<DomainTlsSecurityProfile> {
                    new DomainTlsSecurityProfile(new Domain(1, "domain"), new List<MxRecordTlsSecurityProfile> {mxRecordTlsSecurityProfile1})});

            Assert.That(securityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(securityProfiles[0].Profiles.First(), Is.EqualTo(mxRecordTlsSecurityProfile1));
        }

        [Test]
        public async Task DifferenceInRecordsOldAndNewRecordsReturnedIsReturned()
        {
            MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile1 = CreateTlsSecurityProfile();
            MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile2 = CreateTlsSecurityProfile(2, CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA);

            A.CallTo(() => _tlsSecurityTester.Test(mxRecordTlsSecurityProfile1)).Returns(Task.FromResult(mxRecordTlsSecurityProfile2));

            List<DomainTlsSecurityProfile> securityProfiles = await _tlsSecurityProfileUpdater.UpdateSecurityProfiles(
                new List<DomainTlsSecurityProfile> {
                    new DomainTlsSecurityProfile(new Domain(1, "domain"), new List<MxRecordTlsSecurityProfile> {mxRecordTlsSecurityProfile1})});

            Assert.That(securityProfiles[0].Profiles.Count, Is.EqualTo(2));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.EqualTo(mxRecordTlsSecurityProfile1.TlsSecurityProfile.Id));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Not.Null);
            Assert.That(securityProfiles[0].Profiles[1].TlsSecurityProfile.Id, Is.Null);
            Assert.That(securityProfiles[0].Profiles[1].TlsSecurityProfile.EndDate, Is.Null);
        }

        [Test]
        public async Task EmptyRecordsAreNotReturned()
        {
            MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile1 = CreateTlsSecurityProfile(null);
            MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile2 = CreateTlsSecurityProfile(null, CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA);

            A.CallTo(() => _tlsSecurityTester.Test(mxRecordTlsSecurityProfile1)).Returns(Task.FromResult(mxRecordTlsSecurityProfile2));

            List<DomainTlsSecurityProfile> securityProfiles = await _tlsSecurityProfileUpdater.UpdateSecurityProfiles(
                new List<DomainTlsSecurityProfile> {
                    new DomainTlsSecurityProfile(new Domain(1, "domain"), new List<MxRecordTlsSecurityProfile> {mxRecordTlsSecurityProfile1})});

            Assert.That(securityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Null);
            Assert.That(securityProfiles[0].Profiles[0].MxRecord, Is.EqualTo(mxRecordTlsSecurityProfile1.MxRecord));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.FailureCount, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.FailureCount));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test1Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test1Result));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test2Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test2Result));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test3Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test3Result));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test4Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test4Result));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test5Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test5Result));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test6Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test6Result));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test7Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test7Result));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test8Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test8Result));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test9Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test9Result));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test10Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test10Result));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test11Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test11Result));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Results.Test12Result, Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.Results.Test12Result));
        }

        private MxRecordTlsSecurityProfile CreateTlsSecurityProfile(ulong? id = 1, CipherSuite cipherSuite = CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA)
        {
            TlsTestResult tlsTestResult = new TlsTestResult(TlsVersion.TlsV12, cipherSuite, CurveGroup.Ffdhe2048, SignatureHashAlgorithm.SHA1_DSA, null);

            var tlsSecurityProfile = new TlsSecurityProfile(id, null, new TlsTestResults(0, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult,
                tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, new List<Certificate>
                {
                    new Certificate("thumb", "issuer", "subject", DateTime.Now, DateTime.Now, 1, "alg", "serial", 1, true)
                }));

            return new MxRecordTlsSecurityProfile(new MxRecord(1, "host"), tlsSecurityProfile);
        }
    }
}
