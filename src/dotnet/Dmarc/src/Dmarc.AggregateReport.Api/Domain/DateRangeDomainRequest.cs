using System;

namespace Dmarc.AggregateReport.Api.Domain
{
    public class DateRangeDomainRequest
    {
        public DateTime? BeginDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }
        public int? DomainId { get; set; }
    }
}