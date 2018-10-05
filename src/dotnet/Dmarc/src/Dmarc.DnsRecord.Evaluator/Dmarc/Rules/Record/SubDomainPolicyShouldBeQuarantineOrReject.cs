using System.Linq;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class SubDomainPolicyShouldBeQuarantineOrReject : IRule<DmarcRecord>
    {
        public bool IsErrored(DmarcRecord record, out Error error)
        {
            SubDomainPolicy subDomainPolicy = record.Tags.OfType<SubDomainPolicy>().FirstOrDefault();

            if (IsValid(subDomainPolicy, record.IsOrgDomain))
            {
                error = null;
                return false;
            }

            string errorMessage = string.Format(DmarcRulesResource.SubdomainPolicyMustBeQuarantineOrRejectErrorMessage,
                subDomainPolicy?.PolicyType);

            error = new Error(ErrorType.Warning, errorMessage);
            return true;
        }

        public bool IsValid(SubDomainPolicy subDomainPolicy, bool isOrgDomain)
        {
            // Don't error on unknown because there will already be a parser error for this
            return !isOrgDomain || subDomainPolicy == null || subDomainPolicy.PolicyType != PolicyType.None;
        }
    }
}
