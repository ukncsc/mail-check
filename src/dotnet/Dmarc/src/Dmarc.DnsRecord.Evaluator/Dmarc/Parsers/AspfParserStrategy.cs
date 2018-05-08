using System;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public class AspfParserStrategy : ITagParserStrategy
    {
        public Tag Parse(string tag, string value)
        {
            AlignmentType alignmentType;
            if (!Enum.TryParse(value, true, out alignmentType))
            {
                alignmentType = AlignmentType.Unknown;
            }
            Aspf aspf = new Aspf(tag, alignmentType);

            if (alignmentType == AlignmentType.Unknown)
            {
                string errorMessage = string.Format(DmarcParserResource.InvalidValueErrorMessage, Tag, value);
                aspf.AddError(new Error(ErrorType.Error, errorMessage));
            }

            return aspf;
        }

        public string Tag => "aspf";

        public int MaxOccurences => 1;
    }
}