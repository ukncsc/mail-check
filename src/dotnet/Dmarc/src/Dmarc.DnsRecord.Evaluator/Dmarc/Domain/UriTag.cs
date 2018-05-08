using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class UriTag : DmarcEntity
    {
        public UriTag(string value, DmarcUri uri, MaxReportSize maxReportSize)
        {
            Value = value;
            Uri = uri;
            MaxReportSize = maxReportSize;
        }

        public string Value { get; }
        public DmarcUri Uri { get; }
        public MaxReportSize MaxReportSize { get; }

        public override string ToString()
        {
            return $"{nameof(Value)}: {Value}, {nameof(Uri)}: {Uri}, {nameof(MaxReportSize)}: {MaxReportSize}";
        }

        public override int AllErrorCount => Uri.AllErrorCount + (MaxReportSize?.AllErrorCount ?? 0) + ErrorCount;

        public override IReadOnlyList<Error> AllErrors => Uri.AllErrors.Concat(MaxReportSize?.AllErrors ?? new List<Error>()).Concat(Errors).ToArray();
    }
}