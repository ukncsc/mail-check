using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Dao.Entities;
using Dmarc.MxSecurityTester.MxTester;
using FakeItEasy;
using NUnit.Framework;

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

            A.CallTo(() => _tlsSecurityTester.Test(mxRecordTlsSecurityProfile1))
                .Returns(Task.FromResult(mxRecordTlsSecurityProfile1));

            List<DomainTlsSecurityProfile> securityProfiles = await _tlsSecurityProfileUpdater.UpdateSecurityProfiles(
                new List<DomainTlsSecurityProfile>
                {
                    new DomainTlsSecurityProfile(new Domain(1, "domain"),
                        new List<MxRecordTlsSecurityProfile> {mxRecordTlsSecurityProfile1})
                });

            Assert.That(securityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(securityProfiles[0].Profiles.First(), Is.EqualTo(mxRecordTlsSecurityProfile1));
        }

        [Test]
        public async Task DifferenceInRecordsOldAndNewRecordsReturnedIsReturned()
        {
            MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile1 = CreateTlsSecurityProfile();
            MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile2 =
                CreateTlsSecurityProfile(2, CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA);

            A.CallTo(() => _tlsSecurityTester.Test(mxRecordTlsSecurityProfile1))
                .Returns(Task.FromResult(mxRecordTlsSecurityProfile2));

            List<DomainTlsSecurityProfile> securityProfiles = await _tlsSecurityProfileUpdater.UpdateSecurityProfiles(
                new List<DomainTlsSecurityProfile>
                {
                    new DomainTlsSecurityProfile(new Domain(1, "domain"),
                        new List<MxRecordTlsSecurityProfile> {mxRecordTlsSecurityProfile1})
                });

            Assert.That(securityProfiles[0].Profiles.Count, Is.EqualTo(2));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Id,
                Is.EqualTo(mxRecordTlsSecurityProfile1.TlsSecurityProfile.Id));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Not.Null);
            Assert.That(securityProfiles[0].Profiles[1].TlsSecurityProfile.Id, Is.Null);
            Assert.That(securityProfiles[0].Profiles[1].TlsSecurityProfile.EndDate, Is.Null);
        }

        [Test]
        public async Task EmptyRecordsAreNotReturned()
        {
            MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile1 = CreateTlsSecurityProfile(null);
            MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile2 =
                CreateTlsSecurityProfile(null, CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA);

            A.CallTo(() => _tlsSecurityTester.Test(mxRecordTlsSecurityProfile1))
                .Returns(Task.FromResult(mxRecordTlsSecurityProfile2));

            List<DomainTlsSecurityProfile> securityProfiles = await _tlsSecurityProfileUpdater.UpdateSecurityProfiles(
                new List<DomainTlsSecurityProfile>
                {
                    new DomainTlsSecurityProfile(new Domain(1, "domain"),
                        new List<MxRecordTlsSecurityProfile> {mxRecordTlsSecurityProfile1})
                });

            Assert.That(securityProfiles[0].Profiles.Count, Is.EqualTo(1));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.Id, Is.Null);
            Assert.That(securityProfiles[0].Profiles[0].MxRecord, Is.EqualTo(mxRecordTlsSecurityProfile1.MxRecord));
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.EndDate, Is.Null);
            Assert.That(securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.FailureCount,
                Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.FailureCount));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected,
                Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelected));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
                Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithBestCipherSuiteSelectedFromReverseList));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithSha2HashFunctionSelected,
                Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithSha2HashFunctionSelected));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithWeakCipherSuiteNotSelected,
                Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results
                    .Tls12AvailableWithWeakCipherSuiteNotSelected));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls11AvailableWithBestCipherSuiteSelected,
                Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results
                    .Tls11AvailableWithBestCipherSuiteSelected));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls11AvailableWithWeakCipherSuiteNotSelected,
                Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results
                    .Tls11AvailableWithWeakCipherSuiteNotSelected));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls10AvailableWithBestCipherSuiteSelected,
                Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results
                    .Tls10AvailableWithBestCipherSuiteSelected));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .Tls10AvailableWithWeakCipherSuiteNotSelected,
                Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results
                    .Tls10AvailableWithWeakCipherSuiteNotSelected));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results.Ssl3FailsWithBadCipherSuite,
                Is.EqualTo(
                    mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results.Ssl3FailsWithBadCipherSuite));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results.TlsSecureEllipticCurveSelected,
                Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results
                    .TlsSecureEllipticCurveSelected));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results
                    .TlsSecureDiffieHellmanGroupSelected,
                Is.EqualTo(mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results
                    .TlsSecureDiffieHellmanGroupSelected));
            Assert.That(
                securityProfiles[0].Profiles[0].TlsSecurityProfile.TlsResults.Results.TlsWeakCipherSuitesRejected,
                Is.EqualTo(
                    mxRecordTlsSecurityProfile2.TlsSecurityProfile.TlsResults.Results.TlsWeakCipherSuitesRejected));
        }

        private MxRecordTlsSecurityProfile CreateTlsSecurityProfile(ulong? id = 1,
            CipherSuite cipherSuite = CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA)
        {
            TlsTestResult tlsTestResult = new TlsTestResult(TlsVersion.TlsV12, cipherSuite, CurveGroup.Ffdhe2048,
                SignatureHashAlgorithm.SHA1_DSA, null, null, null);

            var tlsSecurityProfile = new TlsSecurityProfile(id, null, new TlsTestResults(0,
                new TlsTestResultsWithoutCertificate(tlsTestResult,
                    tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult,
                    tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult, tlsTestResult),
                new List<X509Certificate2>
                {
                    TestCertificates.Certificate1
                }));

            return new MxRecordTlsSecurityProfile(new MxRecord(1, "host"), tlsSecurityProfile);
        }
    }
}
