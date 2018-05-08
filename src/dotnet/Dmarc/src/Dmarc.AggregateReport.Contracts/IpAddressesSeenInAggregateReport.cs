using System;
using System.Collections.Generic;

namespace Dmarc.AggregateReport.Contracts
{
    public class AggregateReportIpAddresses : DomainEvent
    {
        public AggregateReportIpAddresses(string correlationId, string causationId,
                DateTime effectiveDate, List<string> ipAddresses)
            : base (correlationId, causationId)
        {
            EffectiveDate = effectiveDate;
            IpAddresses = ipAddresses;
        }

        public DateTime EffectiveDate { get; }
        public List<string> IpAddresses { get; }
    }
}