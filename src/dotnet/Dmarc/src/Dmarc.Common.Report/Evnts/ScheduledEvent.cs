using System;
using System.Collections.Generic;

namespace Dmarc.Common.Report.Evnts
{
    public class ScheduledEvent
    {
        public static ScheduledEvent EmptyScheduledEvent = new ScheduledEvent(string.Empty, string.Empty, new Detail(), string.Empty,
            DateTime.UtcNow, Guid.NewGuid(), new List<string>());

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