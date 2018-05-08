using System.Collections.Generic;

namespace Dmarc.DnsRecord.Contract.Domain
{
    public class DmarcConfig
    {
        public DmarcConfig(Domain domain, List<string> records)
        {
            Domain = domain;
            Records = records;
        }

        public Domain Domain { get; }
        public List<string> Records { get; }
    }
}