using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Caching;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Dao.Entities;
using Dmarc.MxSecurityTester.MxTester;
using FakeItEasy;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Dmarc.MxSecurityTester.Test.MxTester
{
    [TestFixture]
    public class CachingTlsSecurityTesterAdapatorTests
    {
        private CachingTlsSecurityTesterAdapator _cachingTlsSecurityTesterAdaptor;
        private ITlsSecurityTesterAdapator _tlsSecurityTesterAdaptor;
        private ICache _cache;
        private IMxSecurityTesterConfig _config;

        [SetUp]
        public void SetUp()
        {
            _tlsSecurityTesterAdaptor = A.Fake<ITlsSecurityTesterAdapator>();
            _cache = A.Fake<ICache>();
            _config = A.Fake<IMxSecurityTesterConfig>();

            _cachingTlsSecurityTesterAdaptor = new CachingTlsSecurityTesterAdapator(_tlsSecurityTesterAdaptor,
                _cache, _config, A.Fake<ILogger>());
        }

        [Test]
        public async Task CachingNotEnabledCacheNotUsed()
        {
            A.CallTo(() => _config.CachingEnabled).Returns(false);

            MxRecordTlsSecurityProfile securityProfile = CreateSecurityProfile();

            MxRecordTlsSecurityProfile updatedSecurityProfile =
                await _cachingTlsSecurityTesterAdaptor.Test(securityProfile);

            A.CallTo(() => _cache.GetString(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _tlsSecurityTesterAdaptor.Test(securityProfile)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _cache.SetString(A<string>._, A<string>._, A<TimeSpan>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task CachingEnabledAndValueInCacheValueFromCacheReturned()
        {
            MxRecordTlsSecurityProfile securityProfile = CreateSecurityProfile();
            string serializedSecurityProfile =
                JsonConvert.SerializeObject(securityProfile.TlsSecurityProfile.TlsResults);

            A.CallTo(() => _config.CachingEnabled).Returns(true);
            A.CallTo(() => _cache.GetString(A<string>._)).Returns(Task.FromResult(serializedSecurityProfile));

            MxRecordTlsSecurityProfile updatedSecurityProfile =
                await _cachingTlsSecurityTesterAdaptor.Test(securityProfile);

            Assert.That(updatedSecurityProfile.TlsSecurityProfile, Is.EqualTo(securityProfile.TlsSecurityProfile));

            A.CallTo(() => _cache.GetString(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _tlsSecurityTesterAdaptor.Test(securityProfile)).MustNotHaveHappened();
            A.CallTo(() => _cache.SetString(A<string>._, A<string>._, A<TimeSpan>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task CachingEnabledValueNotInCacheValueReturnedFromTlsTesterAndCached()
        {
            MxRecordTlsSecurityProfile securityProfile = CreateSecurityProfile();

            A.CallTo(() => _config.CachingEnabled).Returns(true);
            A.CallTo(() => _cache.GetString(A<string>._)).Returns(Task.FromResult((string) null));
            A.CallTo(() => _tlsSecurityTesterAdaptor.Test(securityProfile)).Returns(Task.FromResult(securityProfile));

            MxRecordTlsSecurityProfile updatedSecurityProfile =
                await _cachingTlsSecurityTesterAdaptor.Test(securityProfile);

            A.CallTo(() => _cache.GetString(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _tlsSecurityTesterAdaptor.Test(securityProfile)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _cache.SetString(A<string>._, A<string>._, A<TimeSpan>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task ErroredResultsNotCachedBeforeThirdFailedRetry()
        {
            MxRecordTlsSecurityProfile securityProfile = CreateSecurityProfile(1);

            A.CallTo(() => _config.CachingEnabled).Returns(true);
            A.CallTo(() => _cache.GetString(A<string>._)).Returns(Task.FromResult((string) null));
            A.CallTo(() => _tlsSecurityTesterAdaptor.Test(securityProfile)).Returns(Task.FromResult(securityProfile));

            MxRecordTlsSecurityProfile updatedSecurityProfile =
                await _cachingTlsSecurityTesterAdaptor.Test(securityProfile);

            A.CallTo(() => _cache.GetString(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _tlsSecurityTesterAdaptor.Test(securityProfile)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _cache.SetString(A<string>._, A<string>._, A<TimeSpan>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task ErroredResultsCachedAfterThirdFailedRetry()
        {
            MxRecordTlsSecurityProfile securityProfile = CreateSecurityProfile(3);

            A.CallTo(() => _config.CachingEnabled).Returns(true);
            A.CallTo(() => _cache.GetString(A<string>._)).Returns(Task.FromResult((string) null));
            A.CallTo(() => _tlsSecurityTesterAdaptor.Test(securityProfile)).Returns(Task.FromResult(securityProfile));

            MxRecordTlsSecurityProfile updatedSecurityProfile =
                await _cachingTlsSecurityTesterAdaptor.Test(securityProfile);

            A.CallTo(() => _cache.GetString(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _tlsSecurityTesterAdaptor.Test(securityProfile)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _cache.SetString(A<string>._, A<string>._, A<TimeSpan>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private MxRecordTlsSecurityProfile CreateSecurityProfile(int failureCount = 0)
        {
            MxRecord mxRecord = new MxRecord(1, "host");

            TlsTestResult tlsTestResult = new TlsTestResult(TlsVersion.TlsV12,
                CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, CurveGroup.Ffdhe2048,
                SignatureHashAlgorithm.SHA1_DSA, null, null, null);

            TlsSecurityProfile tlsSecurityProfile = new TlsSecurityProfile(
                1,
                null,
                new TlsTestResults(
                    failureCount,
                    new TlsTestResultsWithoutCertificate(tlsTestResult,
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
                        tlsTestResult),
                    new List<X509Certificate2> {TestCertificates.Certificate1}
                ));

            return new MxRecordTlsSecurityProfile(mxRecord, tlsSecurityProfile);
        }
    }
}
