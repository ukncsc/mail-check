using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class DmarcRecord : DmarcEntity
    {
        public DmarcRecord(string record, List<Tag> tags, string domain)
        {
            Record = record;
            Tags = tags;
            Domain = domain;
        }

        public string Record { get; }

        public List<Tag> Tags { get; }

        public string Domain { get; }

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
