using System;
using System.Collections.Generic;

namespace Dmarc.DnsRecord.Contract.Domain
{
    public class DmarcConfig
    {
        public DmarcConfig(Domain domain, List<string> records, DateTime lastChecked, string orgDomain, bool isTld, bool isInherited)
        {
            Domain = domain;
            Records = records;
            LastChecked = lastChecked;
            OrgDomain = orgDomain;
            IsTld = isTld;
            IsInherited = isInherited;
        }

        public Domain Domain { get; }
        public List<string> Records { get; }
        public DateTime LastChecked { get; }
        public string OrgDomain { get; }
        public bool IsTld { get; }
        public bool IsInherited { get; }
    }
}