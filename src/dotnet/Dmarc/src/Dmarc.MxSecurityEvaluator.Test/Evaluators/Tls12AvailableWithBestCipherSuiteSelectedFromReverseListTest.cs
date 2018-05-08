using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using NUnit.Framework;

namespace Dmarc.MxSecurityEvaluator.Test.Evaluators
{
    [TestFixture]
    public class Tls12AvailableWithBestCipherSuiteSelectedFromReverseListTest
    {
        private Tls12AvailableWithBestCipherSuiteSelectedFromReverseList sut;

        [SetUp]
        public void SetUp()
        {
            sut = new Tls12AvailableWithBestCipherSuiteSelectedFromReverseList();
        }

        [Test]
        [TestCase(Error.TCP_CONNECTION_FAILED)]
        [TestCase(Error.SESSION_INITIALIZATION_FAILED)]
        public void TcpErrorsShouldResultInInconclusive(Error error)
        {
            var tlsConnectionResult = new TlsConnectionResult(error);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        public void AnErrorShouldResultInAWarning()
        {
            Assert.AreEqual(sut.Test(new TlsConnectionResult(Error.BAD_CERTIFICATE)).Result, EvaluatorResult.WARNING);
        }

        [Test]
        public void NullCipherSuiteShouldResultInInconclusive()
        {
            sut.PreviousCipherSuite = CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA;

            Assert.AreEqual(sut.Test(new TlsConnectionResult(null, null, null, null, null, null)).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        public void UnaccountedForCipherSuiteResponseShouldResultInInconclusive()
        {
            sut.PreviousCipherSuite = CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384;

            var tlsConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        public void PreviousTestBeingInconclusiveShouldResultInPass()
        {
            sut.PreviousResult = new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE);

            var tlsConnection = new TlsConnectionResult(null, CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnection).Result, EvaluatorResult.PASS);
        }

        [Test]
        public void PreviousCipherSuiteIsSameShouldResultInPass()
        {
            sut.PreviousCipherSuite = CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384;

            var tlsConnection = new TlsConnectionResult(null, CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnection).Result, EvaluatorResult.PASS);
        }

        [Test]
        [TestCase(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384)]
        [TestCase(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256)]
        [TestCase(CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384)]
        [TestCase(CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256)]
        [TestCase(CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384)]
        [TestCase(CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256)]
        [TestCase(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384)]
        [TestCase(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256)]
        [TestCase(CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384)]
        [TestCase(CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256)]
        public void PreviousCipherSuiteIsDifferentAndCurrentIsPassShouldResultInPass(CipherSuite cipherSuite)
        {
            sut.PreviousCipherSuite = CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA;

            var tlsConnection = new TlsConnectionResult(null, cipherSuite, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnection).Result, EvaluatorResult.PASS);
        }

        [Test]
        [TestCase(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA)]
        [TestCase(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA)]
        [TestCase(CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA)]
        [TestCase(CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA)]
        [TestCase(CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384)]
        [TestCase(CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256)]
        [TestCase(CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256)]
        [TestCase(CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256)]
        [TestCase(CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA)]
        [TestCase(CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA)]
        [TestCase(CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA)]
        [TestCase(CipherSuite.TLS_RSA_WITH_RC4_128_SHA)]
        public void PreviousCipherSuiteIsDifferentAndCurrentIsWarningShouldResultInWarning(CipherSuite cipherSuite)
        {
            sut.PreviousCipherSuite = CipherSuite.TLS_RSA_WITH_RC4_128_MD5;

            var tlsConnection = new TlsConnectionResult(null, cipherSuite, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnection).Result, EvaluatorResult.WARNING);
        }

        [Test]
        [TestCase(CipherSuite.TLS_RSA_WITH_RC4_128_MD5)]
        [TestCase(CipherSuite.TLS_NULL_WITH_NULL_NULL)]
        [TestCase(CipherSuite.TLS_RSA_WITH_NULL_MD5)]
        [TestCase(CipherSuite.TLS_RSA_WITH_NULL_SHA)]
        [TestCase(CipherSuite.TLS_RSA_EXPORT_WITH_RC4_40_MD5)]
        [TestCase(CipherSuite.TLS_RSA_EXPORT_WITH_RC2_CBC_40_MD5)]
        [TestCase(CipherSuite.TLS_RSA_EXPORT_WITH_DES40_CBC_SHA)]
        [TestCase(CipherSuite.TLS_RSA_WITH_DES_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DH_DSS_EXPORT_WITH_DES40_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DH_DSS_WITH_DES_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DH_RSA_EXPORT_WITH_DES40_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DH_RSA_WITH_DES_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DHE_DSS_WITH_DES_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DHE_RSA_EXPORT_WITH_DES40_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DHE_RSA_WITH_DES_CBC_SHA)]
        public void PreviousCipherSuiteIsDifferentAndCurrentIsFailShouldResultInFail(CipherSuite cipherSuite)
        {
            sut.PreviousCipherSuite = CipherSuite.TLS_RSA_WITH_RC4_128_SHA;

            var tlsConnection = new TlsConnectionResult(null, cipherSuite, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnection).Result, EvaluatorResult.FAIL);
        }
    }
}
