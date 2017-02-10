using System;

namespace Dmarc.AggregateReport.Api.Messages
{
    internal class DateRangeDomainRequest : Request
    {
        public DateRangeDomainRequest(DateTime? beginDateUtc, DateTime? endDateUtc, int? domainId)
        {
            BeginDateUtc = beginDateUtc;
            EndDateUtc = endDateUtc;
            DomainId = domainId;
        }

        public DateTime? BeginDateUtc { get; }
        public DateTime? EndDateUtc { get; }
        public int? DomainId { get; }
    }
}
