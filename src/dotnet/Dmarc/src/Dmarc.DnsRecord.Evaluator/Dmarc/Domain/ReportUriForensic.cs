using System.Collections.Generic;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class ReportUriForensic : ReportUri
    {
        public ReportUriForensic(string value, List<UriTag> uris) 
            : base(value, uris){}
    }
}