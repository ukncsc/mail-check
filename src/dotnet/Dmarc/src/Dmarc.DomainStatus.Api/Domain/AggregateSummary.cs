using System;
using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class AggregateSummary
    {
        public AggregateSummary(SortedDictionary<DateTime, AggregateSummaryItem> results, int totalEmail)
        {
            Results = results;
            TotalEmail = totalEmail;
        }

        public SortedDictionary<DateTime, AggregateSummaryItem> Results { get; }
        public int TotalEmail { get; }
    }
}