using Dmarc.DnsRecord.Evaluator.Explainers;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public class VersionExplainer : IExplainer<Version>
    {
        public bool TryExplain(Version t, out string explanation)
        {
            if (t.Valid)
            {
                explanation = SpfExplainerResource.VersionExplanation;
                return true;
            }

            explanation = null;
            return false;
        }
    }
}