using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public class UnknownModifierExplainer : BaseTermExplainerStrategy<UnknownModifier>
    {
        public override string GetExplanation(UnknownModifier tConcrete)
        {
            return string.Format(SpfExplainerResource.UnknownModifierExplanation, tConcrete.Value);
        }
    }
}