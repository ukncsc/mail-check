using System;
using System.Collections.Generic;

namespace Dmarc.AggregateReport.Api.Dao.Entities
{
    internal class DailyStatistics
    {
        public DailyStatistics(Dictionary<DateTime,Dictionary<string, int>> values)
        {
            Values = values;
        }

        public Dictionary<DateTime, Dictionary<string, int>> Values { get; }
    }
}