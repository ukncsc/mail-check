using System.Collections.Generic;

namespace Dmarc.AggregateReport.Api.Dao.Entities
{
    internal class AggregatedStatistics
    {
        public AggregatedStatistics(Dictionary<string, int> values)
        {
            Values = values;
        }

        public Dictionary<string, int> Values { get; }
    }
}
