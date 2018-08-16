using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class WelcomeSearchResponse
    {
        public WelcomeSearchResponse(WelcomeSearchResult searchResult, bool isPublicSectorOrg)
        {
            SearchResult = searchResult;
            SearchTermIsPublicSectorOrg = isPublicSectorOrg;
        }

        public WelcomeSearchResult SearchResult { get; }

        public bool SearchTermIsPublicSectorOrg { get; }
    }
}
