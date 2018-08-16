using System.Collections.Generic;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using Dmarc.MxSecurityEvaluator.Util;
using NUnit.Framework;

namespace Dmarc.MxSecurityEvaluator.Test.Evaluators
{
    [TestFixture]
    public class TlsWeakCipherSuitesRejectedTest
    {
        private TlsWeakCipherSuitesRejected _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new TlsWeakCipherSuitesRejected();
        }

        [Test]
        public void CorrectTestType()
        {
            Assert.AreEqual(_sut.Type, TlsTestType.TlsWeakCipherSuitesRejected);
        }

        [Test]
        [TestCase(Error.TCP_CONNECTION_FAILED)]
        [TestCase(Error.SESSION_INITIALIZATION_FAILED)]
        public void TcpErrorsShouldResultInInconclusive(Error error)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(error, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsWeakCipherSuitesRejected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        [TestCase(Error.TCP_CONNECTION_FAILED,
            "The server did not present a STARTTLS command with a response code (250)")]
        [TestCase(Error.SESSION_INITIALIZATION_FAILED,
            "The server did not present a STARTTLS command with a response code (250)")]
        public void ErrorsShouldHaveErrorDescriptionInResult(Error error, string description)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(error, description, null);
            ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsWeakCipherSuitesRejected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
            StringAssert.Contains($"Error description \"{description}\".", _sut.Test(connectionResults).Description);
        }

        [Test]
        [TestCase(Error.HANDSHAKE_FAILURE)]
        [TestCase(Error.PROTOCOL_VERSION)]
        [TestCase(Error.INSUFFICIENT_SECURITY)]
        public void ConnectionRefusedErrorsShouldResultInPass(Error error)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(error, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsWeakCipherSuitesRejected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.PASS);
        }

        [Test]
        public void OtherErrorsShouldResultInInconclusive()
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(Error.INTERNAL_ERROR, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsWeakCipherSuitesRejected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        public void NoCipherSuiteResponseShouldResultInInconclusive()
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, null, null, null, null, null, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsWeakCipherSuitesRejected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
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
        public void InsecureCipherSuitesShouldResultInAFail(CipherSuite cipherSuite)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, cipherSuite, null, null, null, null, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsWeakCipherSuitesRejected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.FAIL);
        }
    }
}
