using System.Collections.Generic;

namespace Dmarc.AggregateReport.Api.Domain
{
    public class MatchingDomains
    {
        public MatchingDomains(List<Domain> matches)
        {
            Matches = matches;
        }

        public List<Domain> Matches { get; }
    }
}
