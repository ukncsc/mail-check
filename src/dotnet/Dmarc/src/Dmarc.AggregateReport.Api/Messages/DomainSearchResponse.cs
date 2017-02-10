using Dmarc.AggregateReport.Api.Dao.Entities;

namespace Dmarc.AggregateReport.Api.Messages
{
    internal class DomainSearchResponse : Response
    {
        public DomainSearchResponse(MatchingDomains matches)
        {
            Matches = matches;
        }

        public MatchingDomains Matches { get; }
    }
}
