using System;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class DateRangeDomainRequest
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
