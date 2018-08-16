using System.Collections.Generic;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using Dmarc.MxSecurityEvaluator.Util;
using NUnit.Framework;

namespace Dmarc.MxSecurityEvaluator.Test.Evaluators
{
    [TestFixture]
    public class TlsSecureEllipticCurveSelectedTest
    {
        private TlsSecureEllipticCurveSelected _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new TlsSecureEllipticCurveSelected();
        }

        [Test]
        public void CorrectTestType()
        {
            Assert.AreEqual(_sut.Type, TlsTestType.TlsSecureEllipticCurveSelected);
        }

        [Test]
        [TestCase(Error.TCP_CONNECTION_FAILED)]
        [TestCase(Error.SESSION_INITIALIZATION_FAILED)]
        public void TcpErrorsShouldResultInInconclusive(Error error)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(error, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureEllipticCurveSelected, tlsConnectionResult);

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
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureEllipticCurveSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
            StringAssert.Contains($"Error description \"{description}\".", _sut.Test(connectionResults).Description);
        }

        [Test]
        public void AnErrorShouldResultInInconslusive()
        {
            ConnectionResults connectionResults = TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureEllipticCurveSelected,
                new TlsConnectionResult(Error.CERTIFICATE_UNOBTAINABLE, null, null));

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        public void UnaccountedForCurveShouldResultInInconclusive()
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, null, null, null, null, null, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureEllipticCurveSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        [TestCase(CurveGroup.Unknown)]
        [TestCase(CurveGroup.Secp160k1)]
        [TestCase(CurveGroup.Secp160r1)]
        [TestCase(CurveGroup.Secp160r2)]
        [TestCase(CurveGroup.Secp192k1)]
        [TestCase(CurveGroup.Secp192r1)]
        [TestCase(CurveGroup.Secp224k1)]
        [TestCase(CurveGroup.Secp224r1)]
        [TestCase(CurveGroup.Sect163k1)]
        [TestCase(CurveGroup.Sect163r1)]
        [TestCase(CurveGroup.Sect163r2)]
        [TestCase(CurveGroup.Sect193r1)]
        [TestCase(CurveGroup.Sect193r2)]
        [TestCase(CurveGroup.Sect233k1)]
        [TestCase(CurveGroup.Sect233r1)]
        [TestCase(CurveGroup.Sect239k1)]
        public void CurvesWithCurveNumberLessThan256ShouldResultInAFail(CurveGroup curveGroup)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, null, curveGroup, null, null, null, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureEllipticCurveSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.FAIL);
        }

        [Test]
        [TestCase(CurveGroup.Secp256k1)]
        [TestCase(CurveGroup.Secp256r1)]
        [TestCase(CurveGroup.Secp384r1)]
        [TestCase(CurveGroup.Secp521r1)]
        [TestCase(CurveGroup.Sect283k1)]
        [TestCase(CurveGroup.Sect283r1)]
        [TestCase(CurveGroup.Sect409k1)]
        [TestCase(CurveGroup.Sect409r1)]
        [TestCase(CurveGroup.Sect571k1)]
        [TestCase(CurveGroup.Sect571r1)]
        public void CurvesWithCurveNumberGreaterThan256ShouldResultInAPass(CurveGroup curveGroup)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, null, curveGroup, null, null, null, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureEllipticCurveSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.PASS);
        }
    }
}
