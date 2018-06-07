using System;
using System.Collections.Generic;
using System.Linq;

namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public class SpfConfig : SpfEntity
    {
        public SpfConfig(List<SpfRecord> records, DateTime lastChecked)
        {
            Records = records;
            LastChecked = lastChecked;
        }

        public List<SpfRecord> Records { get; }
        public DateTime LastChecked { get; }

        public override int AllErrorCount => Records.Sum(_ => _.AllErrorCount) + ErrorCount;

        public override IReadOnlyList<Evaluator.Rules.Error> AllErrors => Records.SelectMany(_ => _.AllErrors).Concat(Errors).ToArray();
    }
}