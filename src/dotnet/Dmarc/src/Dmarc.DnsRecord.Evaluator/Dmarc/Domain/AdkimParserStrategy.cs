using System;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class AdkimParserStrategy : ITagParserStrategy
    {
        public Tag Parse(string tag, string value)
        {
            AlignmentType alignmentType;
            if (!Enum.TryParse(value, true, out alignmentType))
            {
                alignmentType = AlignmentType.Unknown;
            }

            Adkim adkim = new Adkim(tag, alignmentType);

            if (alignmentType == AlignmentType.Unknown)
            {
                adkim.AddError(new Error(ErrorType.Error, $"Unknown adkim type: {value}"));
            }

            return adkim;
        }

        public string Tag => "adkim";

        public int MaxOccurences => 1;
    }
}