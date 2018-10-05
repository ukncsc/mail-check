using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class DmarcRecord : DmarcEntity
    {
        public DmarcRecord(string record, List<Tag> tags, string domain, string orgDomain, bool isTld, bool isInherited)
        {
            Record = record;
            Tags = tags;
            Domain = domain;
            OrgDomain = orgDomain;
            IsTld = isTld;
            IsInherited = isInherited;
        }

        public string Record { get; }
        public List<Tag> Tags { get; }
        public string Domain { get; }
        public string OrgDomain { get; }
        public bool IsTld { get; }
        public bool IsInherited { get; }
        public bool IsOrgDomain => string.Equals(Domain, OrgDomain);

        public override string ToString()
        {
            string termsString = string.Join(Environment.NewLine, Tags);
            return $"{nameof(Record)}:{Environment.NewLine}" +
                   $"{Record}{Environment.NewLine}" +
                   $"{nameof(Tags)}{Environment.NewLine}{termsString}";
        }

        public override int AllErrorCount => Tags.Sum(_ => _.AllErrorCount) + ErrorCount;

        public override IReadOnlyList<Error> AllErrors => Tags.SelectMany(_ => _.AllErrors).Concat(Errors).ToArray();
    }
}
