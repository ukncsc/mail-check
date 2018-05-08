using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class DomainsSecurityResponse
    {
        public DomainsSecurityResponse(List<DomainSecurityInfo> domainSecurityInfos, long domainCount)
        {
            DomainSecurityInfos = domainSecurityInfos;
            DomainCount = domainCount;
        }

        public List<DomainSecurityInfo> DomainSecurityInfos { get; }
        public long DomainCount { get; }
    }
}