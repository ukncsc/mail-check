using System;

namespace Dmarc.Metrics.Api.Domain
{
    public class MetricsResults
    {
        public long DmarcAny { get; set; }
        public long DmarcMonitor { get; set; }
        public long DmarcActive { get; set; }
        public long DomainsRegistered { get; set; }
        public long UsersRegistered { get; set; }
        public long DomainsAggregateReporting { get; set; }
        public long AggregateReportsReceived { get; set; }
        public long EmailsBlocked { get; set; }
        public long RuaConfiguredForMailCheck { get; set; }
    }
}
