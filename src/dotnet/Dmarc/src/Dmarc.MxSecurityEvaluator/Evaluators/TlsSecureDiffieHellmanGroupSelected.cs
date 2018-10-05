using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.Common.Util;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Util;

namespace Dmarc.MxSecurityEvaluator.Evaluators
{
    public class TlsSecureDiffieHellmanGroupSelected : ITlsEvaluator
    {
        private readonly string advice = "Only groups of 2048 bits or more should be used.";
        private readonly string intro = "When testing TLS with a range of Diffie Hellman groups {0}";

        public TlsEvaluatorResult Test(ConnectionResults tlsConnectionResults)
        {
            TlsConnectionResult tlsConnectionResult = tlsConnectionResults.TlsSecureDiffieHellmanGroupSelected;

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

            switch (tlsConnectionResult.CurveGroup)
            {
                case CurveGroup.Ffdhe2048:
                case CurveGroup.Ffdhe3072:
                case CurveGroup.Ffdhe4096:
                case CurveGroup.Ffdhe6144:
                case CurveGroup.Ffdhe8192:
                case CurveGroup.UnknownGroup2048:
                case CurveGroup.UnknownGroup3072:
                case CurveGroup.UnknownGroup4096:
                case CurveGroup.UnknownGroup6144:
                case CurveGroup.UnknownGroup8192:
                    return new TlsEvaluatorResult(EvaluatorResult.PASS);

                case CurveGroup.UnknownGroup1024:
                    return new TlsEvaluatorResult(EvaluatorResult.WARNING,
                        string.Format(intro, $"the server selected an unknown 1024 bit group. {advice}"));

                case CurveGroup.Java1024:
                case CurveGroup.Rfc2409_1024:
                case CurveGroup.Rfc5114_1024:
                    return new TlsEvaluatorResult(EvaluatorResult.FAIL,
                        string.Format(intro,
                            $"the server selected {tlsConnectionResult.CurveGroup.GetEnumAsString()} which is an insecure 1024 bit (or less) group. {advice}"));

                case CurveGroup.Unknown:
                    return new TlsEvaluatorResult(EvaluatorResult.FAIL,
                        string.Format(intro,
                            $"the server selected an unknown group which is potentially insecure. {advice}"));
            }

            return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE,
                string.Format(intro, "there was a problem and we are unable to provide additional information."));
        }

        public TlsTestType Type => TlsTestType.TlsSecureDiffieHellmanGroupSelected;
    }
}
