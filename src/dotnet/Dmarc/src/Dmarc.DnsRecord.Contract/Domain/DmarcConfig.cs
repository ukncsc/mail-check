using System;
using System.Collections.Generic;

namespace Dmarc.DnsRecord.Contract.Domain
{
    public class DmarcConfig
    {
        public DmarcConfig(Domain domain, List<string> records, DateTime lastChecked)
        {
            Domain = domain;
            Records = records;
            LastChecked = lastChecked;
        }

        public Domain Domain { get; }
        public List<string> Records { get; }
        public DateTime LastChecked { get; }
    }
}