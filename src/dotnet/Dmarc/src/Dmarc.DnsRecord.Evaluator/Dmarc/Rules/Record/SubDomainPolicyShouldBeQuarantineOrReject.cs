using System.Linq;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class SubDomainPolicyShouldBeQuarantineOrReject : IRule<DmarcRecord>
    {
        private readonly IOrganisationalDomainProvider _organisationalDomainProvider;

        public SubDomainPolicyShouldBeQuarantineOrReject(IOrganisationalDomainProvider organisationalDomainProvider)
        {
            _organisationalDomainProvider = organisationalDomainProvider;
        }

        public bool IsErrored(DmarcRecord record, out Error error)
        {
            SubDomainPolicy subDomainPolicy = record.Tags.OfType<SubDomainPolicy>().FirstOrDefault();
            OrganisationalDomain orgDomain = _organisationalDomainProvider.GetOrganisationalDomain(record.Domain).GetAwaiter().GetResult();

            if (IsValid(subDomainPolicy, orgDomain))
            {
                error = null;
                return false;
            }

            string errorMessage = string.Format(DmarcRulesResource.SubdomainPolicyMustBeQuarantineOrRejectErrorMessage, subDomainPolicy?.PolicyType);

            error = new Error(ErrorType.Warning, errorMessage);
            return true;
        }

        public bool IsValid(SubDomainPolicy subDomainPolicy, OrganisationalDomain orgDomain)
        {
            // Don't error on unknown because there will already be a parser error for this
            return subDomainPolicy == null || subDomainPolicy.PolicyType != PolicyType.None || !orgDomain.IsOrgDomain;
        }
    }
}
