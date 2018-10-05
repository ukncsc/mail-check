using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.Common.Util;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Util;

namespace Dmarc.MxSecurityEvaluator.Evaluators
{
    public class Ssl3FailsWithBadCipherSuite : ITlsEvaluator
    {
        private readonly string advice = "SSL 3.0 is an insecure protocol and should be not supported.";
        private readonly string intro = "When testing SSL 3.0 with a range of cipher suites {0}";

        public TlsEvaluatorResult Test(ConnectionResults tlsConnectionResults)
        {
            TlsConnectionResult tlsConnectionResult = tlsConnectionResults.Ssl3FailsWithBadCipherSuite;

            switch (tlsConnectionResult.Error)
            {
                case Error.HANDSHAKE_FAILURE:
                case Error.PROTOCOL_VERSION:
                case Error.INSUFFICIENT_SECURITY:
                    return new TlsEvaluatorResult(EvaluatorResult.PASS);

                case Error.TCP_CONNECTION_FAILED:
                case Error.SESSION_INITIALIZATION_FAILED:
                    return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, string.Format(intro, $"we were unable to create a connection to the mail server. We will keep trying, so please check back later. Error description \"{tlsConnectionResult.ErrorDescription}\"."));

                case null:
                    break;

                default:
                    return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, string.Format(intro, $"the server responded with an error. Error description \"{tlsConnectionResult.ErrorDescription}\"."));
            }

            string introWithCipherSuite = string.Format(intro,
                $"the server accepted the connection and selected {tlsConnectionResult.CipherSuite.GetEnumAsString()}");

            switch (tlsConnectionResult.CipherSuite)
            {
                case CipherSuite.TLS_RSA_WITH_RC4_128_SHA:
                case CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA:
                    return new TlsEvaluatorResult(EvaluatorResult.WARNING,
                        $"{introWithCipherSuite}. {advice}");

                case CipherSuite.TLS_RSA_WITH_RC4_128_MD5:
                case CipherSuite.TLS_NULL_WITH_NULL_NULL:
                case CipherSuite.TLS_RSA_WITH_NULL_MD5:
                case CipherSuite.TLS_RSA_WITH_NULL_SHA:
                case CipherSuite.TLS_RSA_EXPORT_WITH_RC4_40_MD5:
                case CipherSuite.TLS_RSA_EXPORT_WITH_RC2_CBC_40_MD5:
                case CipherSuite.TLS_RSA_EXPORT_WITH_DES40_CBC_SHA:
                case CipherSuite.TLS_RSA_WITH_DES_CBC_SHA:
                case CipherSuite.TLS_DH_DSS_EXPORT_WITH_DES40_CBC_SHA:
                case CipherSuite.TLS_DH_DSS_WITH_DES_CBC_SHA:
                case CipherSuite.TLS_DH_RSA_EXPORT_WITH_DES40_CBC_SHA:
                case CipherSuite.TLS_DH_RSA_WITH_DES_CBC_SHA:
                case CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA:
                case CipherSuite.TLS_DHE_DSS_WITH_DES_CBC_SHA:
                case CipherSuite.TLS_DHE_RSA_EXPORT_WITH_DES40_CBC_SHA:
                    return new TlsEvaluatorResult(EvaluatorResult.FAIL,
                        $"{introWithCipherSuite} which is insecure. {advice}");
            }

            return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, string.Format(intro, "there was a problem and we are unable to provide additional information."));
        }

        public TlsTestType Type => TlsTestType.Ssl3FailsWithBadCipherSuite;
    }
}
