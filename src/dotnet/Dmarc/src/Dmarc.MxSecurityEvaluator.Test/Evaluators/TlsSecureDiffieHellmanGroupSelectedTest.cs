using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using Dmarc.MxSecurityEvaluator.Util;
using NUnit.Framework;

namespace Dmarc.MxSecurityEvaluator.Test.Evaluators
{
    [TestFixture]
    public class TlsSecureDiffieHellmanGroupSelectedTest
    {
        private TlsSecureDiffieHellmanGroupSelected _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new TlsSecureDiffieHellmanGroupSelected();
        }

        [Test]
        public void CorrectTestType()
        {
            Assert.AreEqual(_sut.Type, TlsTestType.TlsSecureDiffieHellmanGroupSelected);
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
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureDiffieHellmanGroupSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
            StringAssert.Contains($"Error description \"{description}\".", _sut.Test(connectionResults).Description);
        }

        [Test]
        [TestCase(Error.TCP_CONNECTION_FAILED)]
        [TestCase(Error.SESSION_INITIALIZATION_FAILED)]
        public void TcpErrorsShouldResultInInconclusive(Error error)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(error, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureDiffieHellmanGroupSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
        }

        [Test]
        [TestCase(Error.HANDSHAKE_FAILURE)]
        [TestCase(Error.PROTOCOL_VERSION)]
        [TestCase(Error.INSUFFICIENT_SECURITY)]
        public void ConnectionRefusedErrorsShouldResultInPass(Error error)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(error, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureDiffieHellmanGroupSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.PASS);
        }

        [Test]
        public void OtherErrorsShouldResultInInconclusive()
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(Error.INTERNAL_ERROR, null, null);
           ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureDiffieHellmanGroupSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
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
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, null, curveGroup, null, null, null, null, null);
            ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureDiffieHellmanGroupSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.PASS);
        }

        [Test]
        public void Unknown1024GroupShouldResultInAWarn()
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, null, CurveGroup.UnknownGroup1024, null, null, null, null, null);
            ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureDiffieHellmanGroupSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.WARNING);
        }

        [Test]
        [TestCase(CurveGroup.Java1024)]
        [TestCase(CurveGroup.Rfc2409_1024)]
        [TestCase(CurveGroup.Rfc5114_1024)]
        [TestCase(CurveGroup.Unknown)]
        public void Known1024GroupShouldResultInAFail(CurveGroup curveGroup)
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, null, curveGroup, null, null, null, null, null);
            ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureDiffieHellmanGroupSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.FAIL);
        }

        [Test]
        public void NoCurveGroupShouldResultInInconslusive()
        {
            TlsConnectionResult tlsConnectionResult = new TlsConnectionResult(null, null, null, null, null, null, null, null);
            ConnectionResults connectionResults =
                TlsTestDataUtil.CreateConnectionResults(TlsTestType.TlsSecureDiffieHellmanGroupSelected, tlsConnectionResult);

            Assert.AreEqual(_sut.Test(connectionResults).Result, EvaluatorResult.INCONCLUSIVE);
        }
    }
}
