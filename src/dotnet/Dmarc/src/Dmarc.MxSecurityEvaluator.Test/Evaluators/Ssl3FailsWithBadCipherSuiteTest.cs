using System.Collections.Generic;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using Dmarc.MxSecurityEvaluator.Util;
using NUnit.Framework;

namespace Dmarc.MxSecurityEvaluator.Test.Evaluators
{
    [TestFixture]
    public class Ssl3FailsWithBadCipherSuiteTest
    {
        private Ssl3FailsWithBadCipherSuite _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Ssl3FailsWithBadCipherSuite();
        }

        [Test]
        public void CorrectTestType()
        {
            Assert.AreEqual(_sut.Type, TlsTestType.Ssl3FailsWithBadCipherSuite);
        }

        [Test]
        [TestCase(Error.TCP_CONNECTION_FAILED,
            "The server did not present a STARTTLS command with a response code (250)")]
        [TestCase(Error.SESSION_INITIALIZATION_FAILED,
            "The server did not present a STARTTLS command with a response code (250)")]
        [TestCase(Error.BAD_CERTIFICATE,
            "The server returned bad certificate")]
        public void ErrorsShouldHaveErrorDescriptionInResult(Error error, string description)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(error, description, null);
            ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.Ssl3FailsWithBadCipherSuite, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
            StringAssert.Contains($"Error description \"{description}\".", _sut.Test(connectionResults).Description);
        }

        [Test]
        [TestCase(Error.HANDSHAKE_FAILURE, "Handshake failure!")]
        [TestCase(Error.PROTOCOL_VERSION, "Incorrect Protocol version!")]
        [TestCase(Error.INSUFFICIENT_SECURITY, "Insufficient security!")]
        public void ConnectionRefusedErrorsShouldResultInPassWithoutErrorDescription(Error error, string description)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(error, null, null);
            ConnectionResults connectionResults = TlsTestDataUtil.CreateConnectionResults(TlsTestType.Ssl3FailsWithBadCipherSuite, tlsConnectionResult);

            TlsEvaluatorResult result = _sut.Test(connectionResults);

            Assert.AreEqual(result.Result, EvaluatorResult.PASS);
            Assert.That(result.Description, Is.Null);
        }


        [Test]
        [TestCase(Error.TCP_CONNECTION_FAILED)]
        [TestCase(Error.SESSION_INITIALIZATION_FAILED)]
        public void TcpErrorsShouldResultInInconclusive(Error error)
        {
            string errorDescription = "Something went wrong!";
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(error, errorDescription, null);
            ConnectionResults connectionResults = TlsTestDataUtil.CreateConnectionResults(TlsTestType.Ssl3FailsWithBadCipherSuite, tlsConnectionResult);
            TlsEvaluatorResult result = _sut.Test(connectionResults);
            Assert.AreEqual(result.Result, EvaluatorResult.INCONCLUSIVE);
            StringAssert.Contains($"Error description \"{errorDescription}\".", result.Description);
        }

        [Test]
        [TestCase(Error.HANDSHAKE_FAILURE)]
        [TestCase(Error.PROTOCOL_VERSION)]
        [TestCase(Error.INSUFFICIENT_SECURITY)]
        public void ConnectionRefusedErrorsShouldResultInPass(Error error)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(error, null, null);
            ConnectionResults connectionResults = TlsTestDataUtil.CreateConnectionResults(TlsTestType.Ssl3FailsWithBadCipherSuite, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.PASS);
        }

        [Test]
        public void OtherErrorsShouldResultInInconclusive()
        {
            string errorDescription = "Something went wrong!";
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(Error.INTERNAL_ERROR, errorDescription, null);
            ConnectionResults connectionResults =TlsTestDataUtil.CreateConnectionResults(TlsTestType.Ssl3FailsWithBadCipherSuite, tlsConnectionResult);
            TlsEvaluatorResult result = _sut.Test(connectionResults);
            Assert.AreEqual(result.Result, EvaluatorResult.INCONCLUSIVE);
            StringAssert.Contains($"Error description \"{errorDescription}\".", result.Description);
        }

        [Test]
        public void UnaccountedForCipherSuiteResponseShouldResultInInconclusive()
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA, null, null, null, null, null);
            ConnectionResults connectionResults = TlsTestDataUtil.CreateConnectionResults(TlsTestType.Ssl3FailsWithBadCipherSuite, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        [TestCase(CipherSuite.TLS_RSA_WITH_RC4_128_SHA)]
        [TestCase(CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA)]
        [TestCase(CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA)]
        public void NoPfsCipherSuiteShouldResultInWarning(CipherSuite cipherSuite)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, cipherSuite, null, null, null, null, null);
            ConnectionResults connectionResults = TlsTestDataUtil.CreateConnectionResults(TlsTestType.Ssl3FailsWithBadCipherSuite, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.WARNING);
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
        public void InsecureCipherSuitesShouldResultInFail(CipherSuite cipherSuite)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, cipherSuite, null, null, null, null, null);
            ConnectionResults connectionResults = TlsTestDataUtil.CreateConnectionResults(TlsTestType.Ssl3FailsWithBadCipherSuite, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.FAIL);
        }
    }
}
