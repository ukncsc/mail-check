using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public class ExplanationTermExplainer : BaseTermExplainerStrategy<Explanation>
    {
        public override string GetExplanation(Explanation tConcrete)
        {
            return string.Format(SpfExplainerResource.ExplanationExplanation, tConcrete.DomainSpec.Domain);
        }
    }
}