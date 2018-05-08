using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using NUnit.Framework;

namespace Dmarc.MxSecurityEvaluator.Test.Evaluators
{
    [TestFixture]
    public class TlsSecureDiffieHellmanGroupSelectedTest
    {
        private ITlsSecureDiffieHellmanGroupSelected sut;

        [SetUp]
        public void SetUp()
        {
            sut = new TlsSecureDiffieHellmanGroupSelected();
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
        [TestCase(CurveGroup.Ffdhe2048)]
        [TestCase(CurveGroup.Ffdhe3072)]
        [TestCase(CurveGroup.Ffdhe4096)]
        [TestCase(CurveGroup.Ffdhe6144)]
        [TestCase(CurveGroup.Ffdhe8192)]
        [TestCase(CurveGroup.UnknownGroup2048)]
        [TestCase(CurveGroup.UnknownGroup3072)]
        [TestCase(CurveGroup.UnknownGroup4096)]
        [TestCase(CurveGroup.UnknownGroup6144)]
        [TestCase(CurveGroup.UnknownGroup8192)]
        public void GoodCurveGroupsShouldResultInAPass(CurveGroup curveGroup)
        {
            var tlsConnectionResult = new TlsConnectionResult(null, null, curveGroup, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.PASS);
        }

        [Test]
        public void Unknown1024GroupShouldResultInAWarn()
        {
            var tlsConnectionResult = new TlsConnectionResult(null, null, CurveGroup.UnknownGroup1024, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.WARNING);
        }

        [Test]
        [TestCase(CurveGroup.Java1024)]
        [TestCase(CurveGroup.Rfc2409_1024)]
        [TestCase(CurveGroup.Rfc5114_1024)]
        [TestCase(CurveGroup.Unknown)]
        public void Known1024GroupShouldResultInAFail(CurveGroup curveGroup)
        {
            var tlsConnectionResult = new TlsConnectionResult(null, null, curveGroup, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.FAIL);
        }

        [Test]
        public void NoCurveGroupShouldResultInInconslusive()
        {
            var tlsConnectionResult = new TlsConnectionResult(null, null, null, null, null, null);

            Assert.AreEqual(sut.Test(tlsConnectionResult).Result, EvaluatorResult.INCONCLUSIVE);
        }
    }
}
