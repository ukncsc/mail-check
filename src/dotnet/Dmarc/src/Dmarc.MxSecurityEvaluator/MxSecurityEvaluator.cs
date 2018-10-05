using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using Dmarc.MxSecurityEvaluator.Util;

namespace Dmarc.MxSecurityEvaluator
{
    public interface IMxSecurityEvaluator
    {
        EvaluatorResults Evaluate(ConnectionResults tlsConnectionResults);
    }

    public class MxSecurityEvaluator : IMxSecurityEvaluator
    {
        private readonly Dictionary<TlsTestType, ITlsEvaluator> _evaluators;

        public MxSecurityEvaluator(IEnumerable<ITlsEvaluator> evaluators)
        {
            _evaluators = evaluators == null
                ? throw new ArgumentNullException(nameof(evaluators))
                : evaluators.ToDictionary(_ => _.Type);
        }

        public EvaluatorResults Evaluate(ConnectionResults tlsConnectionResults)
        {
            return new EvaluatorResults(
                _evaluators[TlsTestType.Tls12AvailableWithBestCipherSuiteSelected].Test(tlsConnectionResults),
                _evaluators[TlsTestType.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList].Test(tlsConnectionResults),
                _evaluators[TlsTestType.Tls12AvailableWithSha2HashFunctionSelected].Test(tlsConnectionResults),
                _evaluators[TlsTestType.Tls12AvailableWithWeakCipherSuiteNotSelected].Test(tlsConnectionResults),
                _evaluators[TlsTestType.Tls11AvailableWithBestCipherSuiteSelected].Test(tlsConnectionResults),
                _evaluators[TlsTestType.Tls11AvailableWithWeakCipherSuiteNotSelected].Test(tlsConnectionResults),
                _evaluators[TlsTestType.Tls10AvailableWithBestCipherSuiteSelected].Test(tlsConnectionResults),
                _evaluators[TlsTestType.Tls10AvailableWithWeakCipherSuiteNotSelected].Test(tlsConnectionResults),
                _evaluators[TlsTestType.Ssl3FailsWithBadCipherSuite].Test(tlsConnectionResults),
                _evaluators[TlsTestType.TlsSecureEllipticCurveSelected].Test(tlsConnectionResults),
                _evaluators[TlsTestType.TlsSecureDiffieHellmanGroupSelected].Test(tlsConnectionResults),
                _evaluators[TlsTestType.TlsWeakCipherSuitesRejected].Test(tlsConnectionResults)
            );
        }
    }
}
