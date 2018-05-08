using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public class ReportIntervalParserStrategy : ITagParserStrategy
    {
        public Tag Parse(string tag, string value)
        {
            uint candidateInterval;
            uint? interval = null;
            if (uint.TryParse(value, out candidateInterval))
            {
                interval = candidateInterval;
            }

            ReportInterval reportInterval = new ReportInterval(tag, interval);

            if (!interval.HasValue)
            {
                string errorMessage = string.Format(DmarcParserResource.InvalidValueErrorMessage, Tag, value);
                reportInterval.AddError(new Error(ErrorType.Error, errorMessage));
            }

            return reportInterval;
        }

        public string Tag => "ri";

        public int MaxOccurences => 1;
    }
}