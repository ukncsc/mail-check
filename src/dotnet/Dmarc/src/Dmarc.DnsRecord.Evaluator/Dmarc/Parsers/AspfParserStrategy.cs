using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.Common.Util;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public class AspfParserStrategy : ITagParserStrategy
    {
        public Tag Parse(string tag, string value)
        {
            AlignmentType alignmentType;
            if (!value.TryParseExactEnum(out alignmentType))
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
