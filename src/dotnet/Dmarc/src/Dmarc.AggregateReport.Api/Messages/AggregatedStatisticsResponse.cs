using System.Collections.Generic;

namespace Dmarc.AggregateReport.Api.Messages
{
    internal class AggregatedStatisticsResponse : Response
    {
        public AggregatedStatisticsResponse(Dictionary<string,int> values)
        {
            Values = values;
        }

        public Dictionary<string, int> Values { get; }
    }
}
