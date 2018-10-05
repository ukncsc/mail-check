using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Util;

namespace Dmarc.MxSecurityEvaluator.Evaluators
{
    public interface ITlsEvaluator
    {
        TlsEvaluatorResult Test(ConnectionResults tlsConnectionResult);
        TlsTestType Type { get; }
    }
}
