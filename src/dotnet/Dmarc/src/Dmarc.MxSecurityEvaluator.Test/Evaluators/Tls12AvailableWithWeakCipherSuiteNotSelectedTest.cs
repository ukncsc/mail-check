using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using NUnit.Framework;

namespace Dmarc.MxSecurityEvaluator.Test.Evaluators
{
    [TestFixture]
    public class Tls12AvailableWithWeakCipherSuiteNotSelectedTest
    {
        private Tls12AvailableWithWeakCipherSuiteNotSelected sut;

        [SetUp]
        public void SetUp()
        {
            sut = new Tls12AvailableWithWeakCipherSuiteNotSelected();
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
        [TestCase(Error.HANDSHAKE_FAILURE)]
        [TestCase(Error.PROTOCOL_VERSION)]
        [TestCase(Error.INSUFFICIENT_SECURITY)]
        public void ConnectionRefusedErrorsShouldResultInPass(Error error)
        {
            var tlsConnectionResult = new TlsConnectionResult(error);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.PASS);
        }

        [Test]
        public void OtherErrorsShouldResultInInconclusive()
        {
            var tlsConnectionResult = new TlsConnectionResult(Error.INTERNAL_ERROR);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        public void UnaccountedForCipherSuiteResponseShouldResultInInconclusive()
        {
            var tlsConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        [TestCase(CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA)]
        [TestCase(CipherSuite.TLS_RSA_WITH_RC4_128_SHA)]
        [TestCase(CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA)]
        public void GoodCiphersShouldResultInAPass(CipherSuite cipherSuite)
        {
            var tlsConnectionResult = new TlsConnectionResult(null, cipherSuite, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.PASS);
        }

        [Test]
        [TestCase(CipherSuite.TLS_NULL_WITH_NULL_NULL)]
        [TestCase(CipherSuite.TLS_RSA_WITH_NULL_MD5)]
        [TestCase(CipherSuite.TLS_RSA_WITH_RC4_128_MD5)]
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
        public void InsecureCiphersShouldResultInAFail(CipherSuite cipherSuite)
        {
            var tlsConnectionResult = new TlsConnectionResult(null, cipherSuite, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.FAIL);
        }
    }
}
