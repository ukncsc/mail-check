using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class DmarcConfig : DmarcEntity
    {
        public DmarcConfig(List<DmarcRecord> records, string domain, DateTime lastChecked, string orgDomain, bool isTld, bool isInherited)
        {
            Records = records;
            Domain = domain;
            LastChecked = lastChecked;
            OrgDomain = orgDomain;
            IsTld = isTld;
            IsInherited = isInherited;
        }

        public List<DmarcRecord> Records { get; }
        public string Domain { get; }
        public DateTime LastChecked { get; }
        public string OrgDomain { get; }
        public bool IsInherited { get; }
        public bool IsTld { get; }
        
        public override string ToString()
        {
            var recordsString = string.Join(Environment.NewLine, Records);

            return $"{nameof(Records)}:{Environment.NewLine}{recordsString}" +
                $"{(AllErrorCount == 0 ? string.Empty : $"Errors:{Environment.NewLine}{string.Join(Environment.NewLine, AllErrors)}")}";
        }

        public override int AllErrorCount => Records.Sum(_ => _.AllErrorCount) + ErrorCount;

        public override IReadOnlyList<Error> AllErrors => Records.SelectMany(_ => _.AllErrors).Concat(Errors).ToArray();
    }
}
