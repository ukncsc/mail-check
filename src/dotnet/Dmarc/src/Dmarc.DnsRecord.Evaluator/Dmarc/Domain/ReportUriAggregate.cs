using System.Collections.Generic;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class ReportUriAggregate : ReportUri
    {
        public ReportUriAggregate(string value, List<UriTag> uris) 
            : base(value, uris){}
    }
}