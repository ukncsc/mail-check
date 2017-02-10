using System.Collections.Generic;

namespace Dmarc.AggregateReport.Api.Dao.Entities
{
    internal class MatchingDomains
    {
        public MatchingDomains(List<Domain> matches)
        {
            Matches = matches;
        }

        public List<Domain> Matches { get; }
    }
}
