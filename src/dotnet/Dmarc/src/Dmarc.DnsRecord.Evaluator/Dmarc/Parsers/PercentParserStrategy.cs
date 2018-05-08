using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public class PercentParserStrategy : ITagParserStrategy
    {
        public Tag Parse(string tag, string value)
        {
            int candidatePercentValue;
            int? percentValue = null;
            if (int.TryParse(value, out candidatePercentValue) && candidatePercentValue <= 100 && candidatePercentValue >= 0)
            {
                percentValue = candidatePercentValue;
            }

            Percent percent = new Percent(tag, percentValue);

            if (!percentValue.HasValue)
            {
                string errorMessage = string.Format(DmarcParserResource.InvalidValueErrorMessage, Tag, value);
                percent.AddError(new Error(ErrorType.Error, errorMessage));
            }

            return percent;
        }

        public string Tag => "pct";

        public int MaxOccurences => 1;
    }
}