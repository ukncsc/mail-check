using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.Common.Util;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public class SubDomainPolicyParserStrategy : ITagParserStrategy
    {
        public Tag Parse(string tag, string value)
        {
            PolicyType policyType;

            if (!value.TryParseExactEnum(out policyType))
            {
                policyType = PolicyType.Unknown;
            }

            SubDomainPolicy policy = new SubDomainPolicy(tag, policyType);

            if (policyType == PolicyType.Unknown)
            {
                string errorMessage = string.Format(DmarcParserResource.InvalidValueErrorMessage, Tag, value);
                policy.AddError(new Error(ErrorType.Error, errorMessage));
            }

            return policy;
        }

        public string Tag => "sp";

        public int MaxOccurences => 1;
    }
}
