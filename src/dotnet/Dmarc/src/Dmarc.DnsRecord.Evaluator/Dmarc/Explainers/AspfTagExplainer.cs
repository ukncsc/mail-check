using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Explainers
{
    public class AspfTagExplainer : BaseTagExplainerStrategy<Aspf>
    {
        public override string GetExplanation(Aspf aspf)
        {
            return aspf.AlignmentType == AlignmentType.S
                ? DmarcExplainerResource.AspfStrictExplanation
                : DmarcExplainerResource.AspfRelaxedExplanation;
        }
    }
}