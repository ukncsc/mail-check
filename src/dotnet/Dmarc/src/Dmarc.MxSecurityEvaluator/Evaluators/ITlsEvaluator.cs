using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityEvaluator.Evaluators
{
    public interface ITlsEvaluator
    {
        TlsEvaluatorResult Test(TlsConnectionResult tlsConnectionResult);
    }
}
