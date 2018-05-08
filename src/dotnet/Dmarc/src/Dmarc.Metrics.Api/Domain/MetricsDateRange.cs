using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Dmarc.Metrics.Api.Domain
{
    public class MetricsDateRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
