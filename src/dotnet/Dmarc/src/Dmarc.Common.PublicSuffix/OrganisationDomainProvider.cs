using System;
using System.Threading.Tasks;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Louw.PublicSuffix;

namespace Dmarc.Common.PublicSuffix
{
    public class OrganisationDomainProvider : IOrganisationalDomainProvider
    {
        private readonly DomainParser _domainParser;

        public OrganisationDomainProvider()
        {
            WebTldRuleProvider tldRuleProvider = new WebTldRuleProvider(timeToLive: TimeSpan.FromDays(7));

            _domainParser = new DomainParser(tldRuleProvider);
        }

        public async Task<OrganisationalDomain> GetOrganisationalDomain(string domain)
        {
            domain = domain.Trim().TrimEnd('.');

            DomainInfo domainInfo = await _domainParser.ParseAsync(domain);

            return domainInfo == null ?
                new OrganisationalDomain(null, domain, true) : 
                new OrganisationalDomain(domainInfo.RegistrableDomain, domain);
        }
    }
}
