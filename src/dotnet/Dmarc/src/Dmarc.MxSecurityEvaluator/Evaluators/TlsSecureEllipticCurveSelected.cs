using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.Common.Util;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Util;

namespace Dmarc.MxSecurityEvaluator.Evaluators
{
    public class TlsSecureEllipticCurveSelected : ITlsEvaluator
    {
        private readonly string intro = "When testing TLS with a range of elliptic curves {0}";

        public TlsEvaluatorResult Test(ConnectionResults tlsConnectionResults)
        {
            TlsConnectionResult tlsConnectionResult = tlsConnectionResults.TlsSecureEllipticCurveSelected;

            switch (tlsConnectionResult.Error)
            {
                case Error.HANDSHAKE_FAILURE:
                case Error.PROTOCOL_VERSION:
                case Error.INSUFFICIENT_SECURITY:
                    return new TlsEvaluatorResult(EvaluatorResult.PASS);

                case Error.TCP_CONNECTION_FAILED:
                case Error.SESSION_INITIALIZATION_FAILED:
                    return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE,
                        string.Format(intro,
                            $"we were unable to create a connection to the mail server. We will keep trying, so please check back later. Error description \"{tlsConnectionResult.ErrorDescription}\"."));

                case null:
                    break;

                default:
                    return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE,
                        string.Format(intro,
                            $"the server responded with an error. Error description \"{tlsConnectionResult.ErrorDescription}\"."));
            }

            string introWithCurveGroup = string.Format(intro,
                $"the server selected {tlsConnectionResult.CurveGroup.GetEnumAsString()}");

            switch (tlsConnectionResult.CurveGroup)
            {
                case CurveGroup.Unknown:
                case CurveGroup.Secp160k1:
                case CurveGroup.Secp160r1:
                case CurveGroup.Secp160r2:
                case CurveGroup.Secp192k1:
                case CurveGroup.Secp192r1:
                case CurveGroup.Secp224k1:
                case CurveGroup.Secp224r1:
                case CurveGroup.Sect163k1:
                case CurveGroup.Sect163r1:
                case CurveGroup.Sect163r2:
                case CurveGroup.Sect193r1:
                case CurveGroup.Sect193r2:
                case CurveGroup.Sect233k1:
                case CurveGroup.Sect233r1:
                case CurveGroup.Sect239k1:
                    return new TlsEvaluatorResult(EvaluatorResult.FAIL,
                        string.Format(intro, $"the server selected {tlsConnectionResult.CurveGroup.GetEnumAsString()} which has a curve length of less than 256 bits."));

                case CurveGroup.Secp256k1:
                case CurveGroup.Secp256r1:
                case CurveGroup.Secp384r1:
                case CurveGroup.Secp521r1:
                case CurveGroup.Sect283k1:
                case CurveGroup.Sect283r1:
                case CurveGroup.Sect409k1:
                case CurveGroup.Sect409r1:
                case CurveGroup.Sect571k1:
                case CurveGroup.Sect571r1:
                    return new TlsEvaluatorResult(EvaluatorResult.PASS);
            }

            return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE,
                string.Format(intro, "there was a problem and we are unable to provide additional information."));
        }

        public TlsTestType Type => TlsTestType.TlsSecureEllipticCurveSelected;
    }
}
