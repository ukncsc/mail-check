using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.Common.Util;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Util;

namespace Dmarc.MxSecurityEvaluator.Evaluators
{
    public class Tls12AvailableWithSha2HashFunctionSelected : ITlsEvaluator
    {
        private readonly string advice = "Cipher suites with SHA-2 should be selected when presented by the client.";
        private readonly string intro = "When testing TLS 1.2 to ensure the most secure SHA hash function is selected {0}";

        public TlsEvaluatorResult Test(ConnectionResults tlsConnectionResults)
        {
            TlsConnectionResult tlsConnectionResult = tlsConnectionResults.Tls12AvailableWithSha2HashFunctionSelected;

            switch (tlsConnectionResult.Error)
            {
                case Error.TCP_CONNECTION_FAILED:
                case Error.SESSION_INITIALIZATION_FAILED:
                    return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE,
                        string.Format(intro,
                            $"we were unable to create a connection to the mail server. We will keep trying, so please check back later. Error description \"{tlsConnectionResult.ErrorDescription}\"."));

                case null:
                    break;

                default:
                    return new TlsEvaluatorResult(EvaluatorResult.FAIL,
                        string.Format(intro,
                            $"the server responded with an error. Error description \"{tlsConnectionResult.ErrorDescription}\"."));
            }

            string introWithCipherSuite = string.Format(intro, $"the server selected {tlsConnectionResult.CipherSuite.GetEnumAsString()}");

            switch (tlsConnectionResult.CipherSuite)
            {
                case CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384:
                case CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256:
                case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256:
                case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256:
                    return new TlsEvaluatorResult(EvaluatorResult.PASS);

                case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA:
                case CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA:
                    return new TlsEvaluatorResult(EvaluatorResult.WARNING, $"{introWithCipherSuite} which uses SHA-1. {advice}");

                case CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA:
                    return new TlsEvaluatorResult(EvaluatorResult.WARNING, $"{introWithCipherSuite} which uses 3DES and SHA-1. {advice}");

                case CipherSuite.TLS_RSA_WITH_RC4_128_SHA:
                    return new TlsEvaluatorResult(EvaluatorResult.WARNING, $"{introWithCipherSuite} which uses RC4 and SHA-1. {advice}");

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
                case CipherSuite.TLS_DHE_RSA_WITH_DES_CBC_SHA:
                    return new TlsEvaluatorResult(EvaluatorResult.FAIL, $"{introWithCipherSuite} which is insecure. {advice}");
            }

            return new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, string.Format(intro, "there was a problem and we are unable to provide additional information."));
        }

        public TlsTestType Type => TlsTestType.Tls12AvailableWithSha2HashFunctionSelected;
    }
}
