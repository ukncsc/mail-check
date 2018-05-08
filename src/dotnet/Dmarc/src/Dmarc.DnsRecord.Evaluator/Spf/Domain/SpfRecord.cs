using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public class SpfRecord : SpfEntity
    {
        public SpfRecord(string record, Version version, List<Term> terms, string domain)
        {
            Record = record;
            Version = version;
            Terms = terms;
            Domain = domain;
        }

        public string Record { get; }

        public Version Version { get; }

        public List<Term> Terms { get; }

        public string Domain { get; }

        public override int AllErrorCount => Version.AllErrorCount + Terms.Sum(_ => _.AllErrorCount) + ErrorCount;

        public override IReadOnlyList<Error> AllErrors => Version.AllErrors.Concat(Terms.SelectMany(_ => _.AllErrors)).Concat(Errors).ToArray();

        public override string ToString()
        {
            return $"{nameof(Record)}: {Record}{Environment.NewLine}" +
                   $"{nameof(Version)}: {Version}{Environment.NewLine}" +
                   $"{nameof(Terms)}:{Environment.NewLine}" +
                   $"{string.Join(Environment.NewLine, Terms)}{Environment.NewLine}" +
                   $"{(AllValid ? "Valid" : $"Invalid{Environment.NewLine}{string.Join(Environment.NewLine, Errors)}")}";
        }
    }
}
