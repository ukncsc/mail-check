using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public class UnknownTermExplainer : BaseTermExplainerStrategy<UnknownTerm>
    {
        public override string GetExplanation(UnknownTerm tConcrete)
        {
            return string.Format(SpfExplainerResource.UnknownTermExplanation, tConcrete.Value);
        }
    }
}