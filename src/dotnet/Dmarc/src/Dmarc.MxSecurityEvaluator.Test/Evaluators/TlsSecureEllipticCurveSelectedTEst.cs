using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using NUnit.Framework;

namespace Dmarc.MxSecurityEvaluator.Test.Evaluators
{
    [TestFixture]
    public class TlsSecureEllipticCurveSelectedTest
    {
        private TlsSecureEllipticCurveSelected sut;

        [SetUp]
        public void SetUp()
        {
            sut = new TlsSecureEllipticCurveSelected();
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
        public void AnErrorShouldResultInInconslusive()
        {
            Assert.AreEqual(sut.Test(new TlsConnectionResult(Error.CERTIFICATE_UNOBTAINABLE)).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        public void UnaccountedForCurveShouldResultInInconclusive()
        {
            var tlsConnectionResult = new TlsConnectionResult(null, null, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.INCONCLUSIVE);
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
        public void CurvesWithCurveNumberLessThan256ShouldResultInAFail(CurveGroup CurveGroup)
        {
            var tlsConnectionResult = new TlsConnectionResult(null, null, CurveGroup, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.FAIL);
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
        public void CurvesWithCurveNumberGreaterThan256ShouldResultInAPass(CurveGroup CurveGroup)
        {
            var tlsConnectionResult = new TlsConnectionResult(null, null, CurveGroup, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.PASS);
        }
    }
}
