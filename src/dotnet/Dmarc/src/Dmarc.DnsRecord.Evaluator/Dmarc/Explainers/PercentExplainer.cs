using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Explainers
{
    public class PercentExplainer : BaseTagExplainerStrategy<Percent>
    {
        public override string GetExplanation(Percent tConcrete)
        {
            return string.Format(DmarcExplainerResource.PercentExplanation, tConcrete.PercentValue.Value);
        }
    }
}