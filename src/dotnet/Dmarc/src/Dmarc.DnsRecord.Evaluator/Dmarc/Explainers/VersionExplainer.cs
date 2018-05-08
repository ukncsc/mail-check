using Version = Dmarc.DnsRecord.Evaluator.Dmarc.Domain.Version;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Explainers
{
    public class VersionExplainer : BaseTagExplainerStrategy<Version>
    {
        public override string GetExplanation(Version tConcrete)
        {
            return DmarcExplainerResource.VersionDmarc1Explanation;
        }
    }
}