using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Implict
{
    public class SubDomainPolicyImplicitProvider : ImplicitTagProviderStrategyBase<SubDomainPolicy>
    {
        public SubDomainPolicyImplicitProvider() 
            : base(Factory){}

        private static SubDomainPolicy Factory(List<Tag> tags)
        {
            Policy policy = tags.OfType<Policy>().FirstOrDefault();
            return policy == null
                ? null
                : new SubDomainPolicy($"s{policy.Value.Replace(";", string.Empty)}", policy.PolicyType, true);
        }
    }
}