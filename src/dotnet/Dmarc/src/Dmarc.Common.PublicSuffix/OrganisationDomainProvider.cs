using System;
using System.Threading.Tasks;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Louw.PublicSuffix;

namespace Dmarc.Common.PublicSuffix
{
    public class OrganisationDomainProvider : IOrganisationalDomainProvider
    {
        private readonly WebTldRuleProvider _tldRuleProvider;
        private readonly DomainParser _domainParser;

        public OrganisationDomainProvider()
        {
            _tldRuleProvider = new WebTldRuleProvider(timeToLive: TimeSpan.FromDays(7)); //cache data for 10 hours

            _domainParser = new DomainParser(_tldRuleProvider);
        }

        public async Task<OrganisationalDomain> GetOrganisationalDomain(string domain)
        {
            DomainInfo domainInfo = await _domainParser.ParseAsync(domain);

            return new OrganisationalDomain(domainInfo.RegistrableDomain, domain);
        }
    }
}
