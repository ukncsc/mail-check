using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using NUnit.Framework;

namespace Dmarc.MxSecurityEvaluator.Test.Evaluators
{
    [TestFixture]
    public class Tls11AvailableWithFallbackScsvSupportTest
    {
        private Tls11AvailableWithFallbackScsvSupport sut;

        [SetUp]
        public void SetUp()
        {
            sut = new Tls11AvailableWithFallbackScsvSupport();
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
        public void NoTest1ConnectionResultOrTest6ConnectionResultShouldResultInInconclusive()
        {
            var tlsConnectionResult = new TlsConnectionResult(null, null, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        public void NoPreviousCipherSuitesInTest1ResultInAPass()
        {
            sut.Test1ConnectionResult = new TlsConnectionResult(null, null, null, null, null, null);
            sut.Test6ConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, null, null, null, null);

            var tlsConnectionResult = new TlsConnectionResult(null, null, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.PASS);
        }

        [Test]
        public void NoPreviousCipherSuitesInTest6ResultInAPass()
        {
            sut.Test1ConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, null, null, null, null);
            sut.Test6ConnectionResult = new TlsConnectionResult(null, null, null, null, null, null);

            var tlsConnectionResult = new TlsConnectionResult(null, null, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.PASS);
        }

        [Test]
        [TestCase(Error.HANDSHAKE_FAILURE)]
        [TestCase(Error.PROTOCOL_VERSION)]
        [TestCase(Error.INSUFFICIENT_SECURITY)]
        public void ConnectionRefusedErrorsShouldResultInPass(Error error)
        {
            sut.Test1ConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, null, null, null, null);
            sut.Test6ConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, null, null, null, null);

            var tlsConnectionResult = new TlsConnectionResult(error);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.PASS);
        }

        [Test]
        public void OtherErrorsShouldResultInInconclusive()
        {
            sut.Test1ConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, null, null, null, null);
            sut.Test6ConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, null, null, null, null);

            var tlsConnectionResult = new TlsConnectionResult(Error.INTERNAL_ERROR);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        public void AResponseShouldResultInAWarning()
        {
            sut.Test1ConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, null, null, null, null);
            sut.Test6ConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, null, null, null, null);

            var tlsConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.WARNING);
        }

        [Test]
        public void NoResponseOrErrorShouldResultInInconclusive()
        {
            sut.Test1ConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, null, null, null, null);
            sut.Test6ConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA, null, null, null, null);

            var tlsConnectionResult = new TlsConnectionResult(null, null, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.INCONCLUSIVE);
        }
    }
}
