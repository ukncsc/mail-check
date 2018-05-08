using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public abstract class ReportUri : Tag
    {
        protected ReportUri(string value, List<UriTag> uris) : base(value)
        {
            Uris = uris;
        }

        public List<UriTag> Uris { get; }

        public override string ToString()
        {
            string stringUris = string.Join(Environment.NewLine, Uris);
            return $"{base.ToString()}, {nameof(Uris)}:{Environment.NewLine}{stringUris}";
        }

        public override int AllErrorCount => Uris.Sum(_ => _.AllErrorCount) + ErrorCount;

        public override IReadOnlyList<Error> AllErrors => Uris.SelectMany(_ => _.AllErrors).Concat(Errors).ToArray();
    }
}