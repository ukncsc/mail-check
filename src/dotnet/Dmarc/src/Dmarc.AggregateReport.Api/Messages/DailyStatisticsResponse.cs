using System;
using System.Collections.Generic;

namespace Dmarc.AggregateReport.Api.Messages
{
    internal class DailyStatisticsResponse : Response
    {
        public DailyStatisticsResponse(Dictionary<DateTime, Dictionary<string, int>> values)
        {
            Values = values;
        }

        public Dictionary<DateTime, Dictionary<string, int>> Values { get; }
    }
}