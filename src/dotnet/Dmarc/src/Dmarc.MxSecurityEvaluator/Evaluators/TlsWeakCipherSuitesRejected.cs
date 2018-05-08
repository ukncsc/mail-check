using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityEvaluator.Evaluators
{
    public interface ITlsWeakCipherSuitesRejected : ITlsEvaluator { }

    public class TlsWeakCipherSuitesRejected : ITlsWeakCipherSuitesRejected
    {
        private readonly string intro = "When testing TLS with a list of weak cipher suites";

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

            if (tlsConnectionResult.CipherSuite != null)
            {
                return new TlsEvaluatorResult(EvaluatorResult.FAIL, $"{intro} the server accepted the connection.");
            }

            return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, $"{intro} there was a problem and we are unable to provide additional information.");
        }
    }
}
