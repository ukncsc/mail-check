using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class SubDomainPolicyShouldNotBeOnNonOrganisationalDomain : IRule<DmarcRecord>
    {
        private readonly IOrganisationalDomainProvider _organisationalDomainProvider;

        public SubDomainPolicyShouldNotBeOnNonOrganisationalDomain(IOrganisationalDomainProvider organisationalDomainProvider)
        {
            _organisationalDomainProvider = organisationalDomainProvider;
        }

        public bool IsErrored(DmarcRecord record, out Error error)
        {
            SubDomainPolicy subDomainPolicy = record.Tags.OfType<SubDomainPolicy>().FirstOrDefault();
            OrganisationalDomain orgDomain = _organisationalDomainProvider.GetOrganisationalDomain(record.Domain)
                .GetAwaiter().GetResult();

            if (orgDomain?.IsOrgDomain == false && subDomainPolicy != null && !subDomainPolicy.IsImplicit)
            {
                error = new Error(ErrorType.Warning, string.Format(DmarcRulesResource.SubDomainIneffectualErrorMessage, record.Domain));
                return true;
            }

            error = null;
            return false;
        }
    }
}
