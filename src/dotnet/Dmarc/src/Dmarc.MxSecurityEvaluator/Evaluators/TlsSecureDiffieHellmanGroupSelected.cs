using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityEvaluator.Evaluators
{
    public interface ITlsSecureDiffieHellmanGroupSelected : ITlsEvaluator { }

    public class TlsSecureDiffieHellmanGroupSelected : ITlsSecureDiffieHellmanGroupSelected
    {
        private readonly string intro = "When testing TLS with a range of Diffie Hellman groups";

        public TlsEvaluatorResult Test(TlsConnectionResult tlsConnectionResult)
        {
            switch (tlsConnectionResult.Error)
            {
                case Error.HANDSHAKE_FAILURE:
                case Error.PROTOCOL_VERSION:
                case Error.INSUFFICIENT_SECURITY:
                    return new TlsEvaluatorResult(EvaluatorResult.PASS);

                case Error.TCP_CONNECTION_FAILED:
                case Error.SESSION_INITIALIZATION_FAILED:
                    return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, $"{intro} we were unable to create a connection to the mail server. We will keep trying, so please check back later.");

                case null:
                    break;

                default:
                    return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, $"{intro} the server responded with an error.");
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
                    return new TlsEvaluatorResult(EvaluatorResult.WARNING, $"{intro} the server selected a 1024 bit group.");

                case CurveGroup.Java1024:
                case CurveGroup.Rfc2409_1024:
                case CurveGroup.Rfc5114_1024:
                case CurveGroup.Unknown:
                    return new TlsEvaluatorResult(EvaluatorResult.FAIL, $"{intro} the server selected an insecure 1024 bit or less group.");
            }

            return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, $"{intro} there was a problem and we are unable to provide additional information.");
        }
    }
}
