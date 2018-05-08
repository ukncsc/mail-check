using System;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Explainers
{
    public class PolicyExplainer : BaseTagExplainerStrategy<Policy>
    {
        public override string GetExplanation(Policy tConcrete)
        {
            switch (tConcrete.PolicyType)
            {
                case PolicyType.None:
                    return DmarcExplainerResource.PolicyNoneExplanation;
                case PolicyType.Quarantine:
                    return DmarcExplainerResource.PolicyQuarantineExplanation;
                case PolicyType.Reject:
                    return DmarcExplainerResource.PolicyRejectExplanation;
                default:
                    throw new ArgumentException($"Unexpected {nameof(PolicyType)}: {tConcrete.PolicyType}");
            }
        }
    }
}