using System;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Explainers
{
    public class SubDomainPolicyExplainer : BaseTagExplainerStrategy<SubDomainPolicy>
    {
        public override string GetExplanation(SubDomainPolicy tConcrete)
        {
            switch (tConcrete.PolicyType)
            {
                case PolicyType.None:
                    return DmarcExplainerResource.SubDomainPolicyNoneExplanation;
                case PolicyType.Quarantine:
                    return DmarcExplainerResource.SubDomainPolicyQuarantineExplanation;
                case PolicyType.Reject:
                    return DmarcExplainerResource.SubDomainPolicyRejectExplanation;
                default:
                    throw new ArgumentException($"Unexpected {nameof(PolicyType)}: {tConcrete.PolicyType}");
            }
        }
    }
}