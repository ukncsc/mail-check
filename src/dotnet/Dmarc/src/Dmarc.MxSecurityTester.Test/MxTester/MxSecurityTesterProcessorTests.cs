using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Dao;
using Dmarc.MxSecurityTester.Dao.Entities;
using Dmarc.MxSecurityTester.MxTester;
using FakeItEasy;
using NUnit.Framework;
using Certificate = Dmarc.MxSecurityTester.Dao.Entities.Certificate;

namespace Dmarc.MxSecurityTester.Test.MxTester
{
    [TestFixture]
    public class MxSecurityTesterProcessorTests
    {
        private MxSecurityTesterProcessor _mxSecurityTesterProcessor;
        private IDomainTlsSecurityProfileDao _domainTlsSecurityProfileDao;
        private IPublishingTlsSecurityProfileUpdater _tlsSecurityProfileUpdater;

        [SetUp]
        public void SetUp()
        {
            _domainTlsSecurityProfileDao = A.Fake<IDomainTlsSecurityProfileDao>();
            _tlsSecurityProfileUpdater = A.Fake<IPublishingTlsSecurityProfileUpdater>();

            _mxSecurityTesterProcessor = new MxSecurityTesterProcessor(_domainTlsSecurityProfileDao,
                _tlsSecurityProfileUpdater, A.Fake<ILogger>());
        }

        [Test]
        public async Task NoProfileToProcessNoProcessingOccurs()
        {
            A.CallTo(() => _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate()).Returns(Task.FromResult(new List<DomainTlsSecurityProfile>()));

            await _mxSecurityTesterProcessor.Process();

            A.CallTo(() => _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _tlsSecurityProfileUpdater.UpdateSecurityProfiles(A<List<DomainTlsSecurityProfile>>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task ProcessingStopsWhenNoMoreProfilesToProcess()
        {
            A.CallTo(() => _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate()).ReturnsNextFromSequence(
                Task.FromResult(new List<DomainTlsSecurityProfile> { CreateDomainTlsSecurityProfile() }),
                Task.FromResult(new List<DomainTlsSecurityProfile>())
                );

            await _mxSecurityTesterProcessor.Process();

            A.CallTo(() => _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate()).MustHaveHappened(Repeated.Exactly.Twice);
            A.CallTo(() => _tlsSecurityProfileUpdater.UpdateSecurityProfiles(A<List<DomainTlsSecurityProfile>>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        private DomainTlsSecurityProfile CreateDomainTlsSecurityProfile()
        {
            TlsTestResult tlsTestResult = new TlsTestResult(TlsVersion.TlsV12, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, CurveGroup.Ffdhe2048, SignatureHashAlgorithm.SHA1_DSA, null);

            TlsSecurityProfile tlsSecurityProfile = new TlsSecurityProfile(
                1,
                null,
                new TlsTestResults(0,
                tlsTestResult,
                tlsTestResult,
                tlsTestResult,
                tlsTestResult,
                tlsTestResult,
                tlsTestResult,
                tlsTestResult,
                tlsTestResult,
                tlsTestResult,
                tlsTestResult,
                tlsTestResult,
                tlsTestResult,
                tlsTestResult,
                new List<Certificate>()
            ));

            return new DomainTlsSecurityProfile(new Domain(1, "domain"),
                new List<MxRecordTlsSecurityProfile> { new MxRecordTlsSecurityProfile(new MxRecord(1, "host"), tlsSecurityProfile) });
        }
    }
}
