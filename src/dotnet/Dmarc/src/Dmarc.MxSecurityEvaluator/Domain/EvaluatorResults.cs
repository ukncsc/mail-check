using Dmarc.Common.Interface.Tls.Domain;
using Newtonsoft.Json;

namespace Dmarc.MxSecurityEvaluator.Domain
{
    public class EvaluatorResults
    {
        public EvaluatorResults(TlsEvaluatorResult tls12AvailableWithBestCipherSuiteSelected,
            TlsEvaluatorResult tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
            TlsEvaluatorResult tls12AvailableWithSha2HashFunctionSelected,
            TlsEvaluatorResult tls12AvailableWithWeakCipherSuiteNotSelected,
            TlsEvaluatorResult tls11AvailableWithBestCipherSuiteSelected,
            TlsEvaluatorResult tls11AvailableWithWeakCipherSuiteNotSelected,
            TlsEvaluatorResult tls10AvailableWithBestCipherSuiteSelected,
            TlsEvaluatorResult tls10AvailableWithWeakCipherSuiteNotSelected,
            TlsEvaluatorResult ssl3FailsWithBadCipherSuite,
            TlsEvaluatorResult tlsSecureEllipticCurveSelected,
            TlsEvaluatorResult tlsSecureDiffieHellmanGroupSelected,
            TlsEvaluatorResult tlsWeakCipherSuitesRejected)
        {
            Tls12AvailableWithBestCipherSuiteSelected = tls12AvailableWithBestCipherSuiteSelected;
            Tls12AvailableWithBestCipherSuiteSelectedFromReverseList = tls12AvailableWithBestCipherSuiteSelectedFromReverseList;
            Tls12AvailableWithSha2HashFunctionSelected = tls12AvailableWithSha2HashFunctionSelected;
            Tls12AvailableWithWeakCipherSuiteNotSelected = tls12AvailableWithWeakCipherSuiteNotSelected;
            Tls11AvailableWithBestCipherSuiteSelected = tls11AvailableWithBestCipherSuiteSelected;
            Tls11AvailableWithWeakCipherSuiteNotSelected = tls11AvailableWithWeakCipherSuiteNotSelected;
            Tls10AvailableWithBestCipherSuiteSelected = tls10AvailableWithBestCipherSuiteSelected;
            Tls10AvailableWithWeakCipherSuiteNotSelected = tls10AvailableWithWeakCipherSuiteNotSelected;
            Ssl3FailsWithBadCipherSuite = ssl3FailsWithBadCipherSuite;
            TlsSecureEllipticCurveSelected = tlsSecureEllipticCurveSelected;
            TlsSecureDiffieHellmanGroupSelected = tlsSecureDiffieHellmanGroupSelected;
            TlsWeakCipherSuitesRejected = tlsWeakCipherSuitesRejected;
        }


        public TlsEvaluatorResult Tls12AvailableWithBestCipherSuiteSelected { get; }
        public TlsEvaluatorResult Tls12AvailableWithBestCipherSuiteSelectedFromReverseList { get; }
        public TlsEvaluatorResult Tls12AvailableWithSha2HashFunctionSelected { get; }
        public TlsEvaluatorResult Tls12AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsEvaluatorResult Tls11AvailableWithBestCipherSuiteSelected { get; }
        public TlsEvaluatorResult Tls11AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsEvaluatorResult Tls10AvailableWithBestCipherSuiteSelected { get; }
        public TlsEvaluatorResult Tls10AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsEvaluatorResult Ssl3FailsWithBadCipherSuite { get; }
        public TlsEvaluatorResult TlsSecureEllipticCurveSelected { get; }
        public TlsEvaluatorResult TlsSecureDiffieHellmanGroupSelected { get; }
        public TlsEvaluatorResult TlsWeakCipherSuitesRejected { get; }

        public static EvaluatorResults GetConnectionFailedResults(string errorDescription)
        {
            string errorMessage =
                "We were unable to create a TLS connection with this server. This could be because the server does not support " +
                "TLS or because Mail Check servers have been blocked. We will keep trying to test TLS with this server, " +
                $"so please check back later or get in touch if you think there's a problem.";

            if (!string.IsNullOrWhiteSpace(errorDescription))
            {
                errorMessage += $" Error description \"{errorDescription}\".";
            }

            TlsEvaluatorResult tlsEvaluatorResult = new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE, errorMessage);

            return CreateSingleTlsResult(tlsEvaluatorResult);
        }

        public static EvaluatorResults GetHostNotFoundResults(string hostname)
        {
            TlsEvaluatorResult tlsEvaluatorResult = new TlsEvaluatorResult(EvaluatorResult.FAIL,
                $"This hostname {hostname} does not exist.");

            return CreateSingleTlsResult(tlsEvaluatorResult);
        }

        [JsonIgnore]
        public static EvaluatorResults EmptyResults =>
            new EvaluatorResults(new TlsEvaluatorResult(), new TlsEvaluatorResult(),
                new TlsEvaluatorResult(), new TlsEvaluatorResult(), new TlsEvaluatorResult(),
                new TlsEvaluatorResult(), new TlsEvaluatorResult(), new TlsEvaluatorResult(),
                new TlsEvaluatorResult(), new TlsEvaluatorResult(), new TlsEvaluatorResult(),
                new TlsEvaluatorResult());

        private static EvaluatorResults CreateSingleTlsResult(TlsEvaluatorResult tlsEvaluatorResult) =>
            new EvaluatorResults(tlsEvaluatorResult,
                new TlsEvaluatorResult(EvaluatorResult.PASS), new TlsEvaluatorResult(EvaluatorResult.PASS),
                new TlsEvaluatorResult(EvaluatorResult.PASS), new TlsEvaluatorResult(EvaluatorResult.PASS),
                new TlsEvaluatorResult(EvaluatorResult.PASS), new TlsEvaluatorResult(EvaluatorResult.PASS),
                new TlsEvaluatorResult(EvaluatorResult.PASS), new TlsEvaluatorResult(EvaluatorResult.PASS),
                new TlsEvaluatorResult(EvaluatorResult.PASS), new TlsEvaluatorResult(EvaluatorResult.PASS),
                new TlsEvaluatorResult(EvaluatorResult.PASS));
    }
}