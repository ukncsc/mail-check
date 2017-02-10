using System;
using System.Collections.Generic;

namespace Dmarc.AggregateReport.Parser.Lambda.Evnts
{
    public class ScheduledEvent
    {
        public ScheduledEvent(string account, string region, Detail detail, string source, DateTime time, Guid id, List<string> resources)
        {
            Account = account;
            Region = region;
            Detail = detail;
            Source = source;
            Time = time;
            Id = id;
            Resources = resources;
        }

        public string Account { get; }
        public string Region { get; }
        public Detail Detail { get; }
        public string Source { get; }
        public DateTime Time { get; }
        public Guid Id { get; }
        public List<string> Resources { get; }
    }
}