using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityEvaluator.Evaluators
{
    public interface ITls11AvailableWithFallbackScsvSupport : ITlsEvaluator
    {
        TlsConnectionResult Test1ConnectionResult { get; set; }
        TlsConnectionResult Test6ConnectionResult { get; set; }
    }

    public class Tls11AvailableWithFallbackScsvSupport : ITls11AvailableWithFallbackScsvSupport
    {
        private readonly string advice = "The server should support SCSV Fallback Signaling in order to prevent a downgrade attack.";
        private readonly string intro = "When testing TLS 1.1 with SCSV Fallback Signaling";

        public TlsConnectionResult Test1ConnectionResult { get; set; }

        public TlsConnectionResult Test6ConnectionResult { get; set; }

        public TlsEvaluatorResult Test(TlsConnectionResult tlsConnectionResult)
        {
            if (Test1ConnectionResult == null || Test6ConnectionResult == null)
            {
                return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, $"{intro} there was a problem and we are unable to provide additional information.");
            }

            if (Test1ConnectionResult.CipherSuite == null || Test6ConnectionResult.CipherSuite == null)
            {
                return new TlsEvaluatorResult(EvaluatorResult.PASS);
            }

            switch (tlsConnectionResult.Error)
            {
                case Error.HANDSHAKE_FAILURE:
                case Error.PROTOCOL_VERSION:
                case Error.INAPPROPRIATE_FALLBACK:
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

            if (tlsConnectionResult.CipherSuite != null)
            {
                return new TlsEvaluatorResult(EvaluatorResult.WARNING, $"{intro} the server did not refuse the connection. {advice}");
            }

            return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, $"{intro} there was a problem and we are unable to provide additional information.");
        }
    }
}
