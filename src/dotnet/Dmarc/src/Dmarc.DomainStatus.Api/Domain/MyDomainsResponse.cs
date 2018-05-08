using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class MyDomainsResponse : DomainsSecurityResponse
    {
        public MyDomainsResponse(List<DomainSecurityInfo> domainSecurityInfos, long domainCount, long userDomainCount) 
            : base(domainSecurityInfos, domainCount)
        {
            UserDomainCount = userDomainCount;
        }

        public long UserDomainCount { get; }
    }
}