using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using System.Collections.Generic;

namespace Dmarc.MxSecurityEvaluator
{
    public interface IMxSecurityEvaluator
    {
        List<TlsEvaluatorResult> Evaluate(List<TlsConnectionResult> tlsConnectionResults);
    }

    public class MxSecurityEvaluator : IMxSecurityEvaluator
    {
        private readonly ISsl3FailsWithBadCipherSuite _ssl3FailsWithBadCipherSuite;
        private readonly ITls10AvailableWithBestCipherSuiteSelected _tls10AvailableWithBestCipherSuiteSelected;
        private readonly ITls10AvailableWithWeakCipherSuiteNotSelected _tls10AvailableWithWeakCipherSuiteNotSelected;
        private readonly ITls11AvailableWithBestCipherSuiteSelected _tls11AvailableWithBestCipherSuiteSelected;
        private readonly ITls11AvailableWithFallbackScsvSupport _tls11AvailableWithFallbackScsvSupport;
        private readonly ITls11AvailableWithWeakCipherSuiteNotSelected _tls11AvailableWithWeakCipherSuiteNotSelected;
        private readonly ITls12AvailableWithBestCipherSuiteSelected _tls12AvailableWithBestCipherSuiteSelected;
        private readonly ITls12AvailableWithBestCipherSuiteSelectedFromReverseList _tls12AvailableWithBestCipherSuiteSelectedFromReverseList;
        private readonly ITls12AvailableWithSha2HashFunctionSelected _tls12AvailableWithSha2HashFunctionSelected;
        private readonly ITls12AvailableWithWeakCipherSuiteNotSelected _tls12AvailableWithWeakCipherSuiteNotSelected;
        private readonly ITlsSecureDiffieHellmanGroupSelected _tlsSecureDiffieHellmanGroupSelected;
        private readonly ITlsSecureEllipticCurveSelected _tlsSecureEllipticCurveSelected;
        private readonly ITlsWeakCipherSuitesRejected _tlsWeakCipherSuitesRejected;

        public MxSecurityEvaluator(
            ISsl3FailsWithBadCipherSuite ssl3FailsWithBadCipherSuite,
            ITls10AvailableWithBestCipherSuiteSelected tls10AvailableWithBestCipherSuiteSelected,
            ITls10AvailableWithWeakCipherSuiteNotSelected tls10AvailableWithWeakCipherSuiteNotSelected,
            ITls11AvailableWithBestCipherSuiteSelected tls11AvailableWithBestCipherSuiteSelected,
            ITls11AvailableWithWeakCipherSuiteNotSelected tls11AvailableWithWeakCipherSuiteNotSelected,
            ITls12AvailableWithBestCipherSuiteSelected tls12AvailableWithBestCipherSuiteSelected,
            ITls12AvailableWithBestCipherSuiteSelectedFromReverseList tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
            ITls11AvailableWithFallbackScsvSupport tls12AvailableWithFallbackScsvSupport,
            ITls12AvailableWithSha2HashFunctionSelected tls12AvailableWithSha2HashFunctionSelected,
            ITls12AvailableWithWeakCipherSuiteNotSelected tls12AvailableWithWeakCipherSuiteNotSelected,
            ITlsSecureDiffieHellmanGroupSelected tlsSecureDiffieHellmanGroupSelected,
            ITlsSecureEllipticCurveSelected tlsSecureEllipticCurveSelected,
            ITlsWeakCipherSuitesRejected tlsWeakCipherSuitesRejected)
        {
            _ssl3FailsWithBadCipherSuite = ssl3FailsWithBadCipherSuite;
            _tls10AvailableWithBestCipherSuiteSelected = tls10AvailableWithBestCipherSuiteSelected;
            _tls10AvailableWithWeakCipherSuiteNotSelected = tls10AvailableWithWeakCipherSuiteNotSelected;
            _tls11AvailableWithBestCipherSuiteSelected = tls11AvailableWithBestCipherSuiteSelected;
            _tls11AvailableWithWeakCipherSuiteNotSelected = tls11AvailableWithWeakCipherSuiteNotSelected;
            _tls12AvailableWithBestCipherSuiteSelected = tls12AvailableWithBestCipherSuiteSelected;
            _tls12AvailableWithBestCipherSuiteSelectedFromReverseList = tls12AvailableWithBestCipherSuiteSelectedFromReverseList;
            _tls11AvailableWithFallbackScsvSupport = tls12AvailableWithFallbackScsvSupport;
            _tls12AvailableWithSha2HashFunctionSelected = tls12AvailableWithSha2HashFunctionSelected;
            _tls12AvailableWithWeakCipherSuiteNotSelected = tls12AvailableWithWeakCipherSuiteNotSelected;
            _tlsSecureDiffieHellmanGroupSelected = tlsSecureDiffieHellmanGroupSelected;
            _tlsSecureEllipticCurveSelected = tlsSecureEllipticCurveSelected;
            _tlsWeakCipherSuitesRejected = tlsWeakCipherSuitesRejected;
        }

        public List<TlsEvaluatorResult> Evaluate(List<TlsConnectionResult> tlsConnectionResults)
        {
            if (tlsConnectionResults.Count >= 13)
            {
                var evaluatorResults = new List<TlsEvaluatorResult>();

                evaluatorResults.Add(_tls12AvailableWithBestCipherSuiteSelected.Test(tlsConnectionResults[0]));

                // provide second test with connection results and evaluator results of the first
                _tls12AvailableWithBestCipherSuiteSelectedFromReverseList.PreviousCipherSuite = tlsConnectionResults[0].CipherSuite;
                _tls12AvailableWithBestCipherSuiteSelectedFromReverseList.PreviousResult = evaluatorResults[0];
                evaluatorResults.Add(_tls12AvailableWithBestCipherSuiteSelectedFromReverseList.Test(tlsConnectionResults[1]));

                evaluatorResults.Add(_tls12AvailableWithSha2HashFunctionSelected.Test(tlsConnectionResults[2]));
                evaluatorResults.Add(_tls12AvailableWithWeakCipherSuiteNotSelected.Test(tlsConnectionResults[3]));

                // provide fifth test with connection results of the first and sixth
                _tls11AvailableWithFallbackScsvSupport.Test1ConnectionResult = tlsConnectionResults[0];
                _tls11AvailableWithFallbackScsvSupport.Test6ConnectionResult = tlsConnectionResults[5];
                evaluatorResults.Add(_tls11AvailableWithFallbackScsvSupport.Test(tlsConnectionResults[4]));

                evaluatorResults.Add(_tls11AvailableWithBestCipherSuiteSelected.Test(tlsConnectionResults[5]));
                evaluatorResults.Add(_tls11AvailableWithWeakCipherSuiteNotSelected.Test(tlsConnectionResults[6]));
                evaluatorResults.Add(_tls10AvailableWithBestCipherSuiteSelected.Test(tlsConnectionResults[7]));
                evaluatorResults.Add(_tls10AvailableWithWeakCipherSuiteNotSelected.Test(tlsConnectionResults[8]));
                evaluatorResults.Add(_ssl3FailsWithBadCipherSuite.Test(tlsConnectionResults[9]));
                evaluatorResults.Add(_tlsSecureEllipticCurveSelected.Test(tlsConnectionResults[10]));
                evaluatorResults.Add(_tlsSecureDiffieHellmanGroupSelected.Test(tlsConnectionResults[11]));
                evaluatorResults.Add(_tlsWeakCipherSuitesRejected.Test(tlsConnectionResults[12]));

                return evaluatorResults;
            }

            return null;
        }
    }
}
