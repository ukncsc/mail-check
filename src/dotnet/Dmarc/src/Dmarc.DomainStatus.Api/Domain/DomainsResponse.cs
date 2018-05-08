using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class DomainsResponse
    {
        public DomainsResponse(List<DomainStatus> domains, long domainCount, int page, int pageSize, string search)
        {
            Domains = domains;
            DomainCount = domainCount;
            Page = page;
            PageSize = pageSize;
            Search = search;
        }

        public List<DomainStatus> Domains { get; }
        public long DomainCount { get; }
        public int Page { get; }
        public int PageSize { get; }
        public string Search { get; }
    }
}
