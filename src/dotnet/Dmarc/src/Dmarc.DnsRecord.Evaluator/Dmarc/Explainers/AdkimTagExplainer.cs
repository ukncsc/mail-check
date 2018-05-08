using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Explainers
{
    public class AdkimExplainer : BaseTagExplainerStrategy<Adkim>
    {
        public override string GetExplanation(Adkim tConcrete)
        {
            return tConcrete.AlignmentType == AlignmentType.S
                ? DmarcExplainerResource.AdkimStrictExplanation
                : DmarcExplainerResource.AdkimRelaxedExplanation;
        }
    }
}