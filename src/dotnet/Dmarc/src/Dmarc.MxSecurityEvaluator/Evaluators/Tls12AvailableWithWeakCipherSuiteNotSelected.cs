using System.Collections.Generic;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Util;

namespace Dmarc.MxSecurityEvaluator.Evaluators
{
    public class Tls12AvailableWithWeakCipherSuiteNotSelected : ITlsEvaluator
    {
        private readonly string intro = "When testing TLS 1.2 with a range of weak cipher suites";

        public TlsEvaluatorResult Test(ConnectionResults tlsConnectionResults)
        {
            TlsConnectionResult tlsConnectionResult = tlsConnectionResults.Tls12AvailableWithWeakCipherSuiteNotSelected;

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

            switch (tlsConnectionResult.CipherSuite)
            {
                case CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_RSA_WITH_RC4_128_SHA:
                case CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA:
                    return new TlsEvaluatorResult(EvaluatorResult.PASS);

                case CipherSuite.TLS_NULL_WITH_NULL_NULL:
                case CipherSuite.TLS_RSA_WITH_NULL_MD5:
                case CipherSuite.TLS_RSA_WITH_RC4_128_MD5:
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
                case CipherSuite.TLS_DHE_RSA_WITH_DES_CBC_SHA:
                    return new TlsEvaluatorResult(EvaluatorResult.FAIL, $"{intro} the server did not refuse the connection and selected an insecure cipher suite.");
            }

            return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, $"{intro} there was a problem and we are unable to provide additional information.");
        }

        public TlsTestType Type => TlsTestType.Tls12AvailableWithWeakCipherSuiteNotSelected;
    }
}
