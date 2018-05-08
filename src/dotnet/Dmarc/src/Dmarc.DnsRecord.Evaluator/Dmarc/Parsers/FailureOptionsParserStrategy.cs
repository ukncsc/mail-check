using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public class FailureOptionsParserStrategy : ITagParserStrategy
    {
        public Tag Parse(string tag, string value)
        {
            FailureOptionType failureOptionType;
            switch (value?.ToLower())
            {
                case "0":
                    failureOptionType = FailureOptionType.Zero;
                    break;
                case "1":
                    failureOptionType = FailureOptionType.One;
                    break;
                case "d":
                    failureOptionType = FailureOptionType.D;
                    break;
                case "s":
                    failureOptionType = FailureOptionType.S;
                    break;
                default:
                    failureOptionType = FailureOptionType.Unknown;
                    break;
            }
            FailureOption failureOption = new FailureOption(tag, failureOptionType);

            if (failureOptionType == FailureOptionType.Unknown)
            {
                string errorMessage = string.Format(DmarcParserResource.InvalidValueErrorMessage, Tag, value);
                failureOption.AddError(new Error(ErrorType.Error, errorMessage));
            }

            return failureOption;
        }

        public string Tag => "fo";

        public int MaxOccurences => 1;
    }
}