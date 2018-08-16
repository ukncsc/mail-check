using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.Common.Util;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public class ReportFormatParserStrategy : ITagParserStrategy
    {
        public Tag Parse(string tag, string value)
        {
            ReportFormatType reportFormatType;
            if (!value.TryParseExactEnum(out reportFormatType))
            {
                reportFormatType = ReportFormatType.Unknown;
            }

            ReportFormat reportFormat = new ReportFormat(tag, reportFormatType);

            if (reportFormatType == ReportFormatType.Unknown)
            {
                string errorMessage = string.Format(DmarcParserResource.InvalidValueErrorMessage, Tag, value);
                reportFormat.AddError(new Error(ErrorType.Error, errorMessage));
            }

            return reportFormat;
        }

        public string Tag => "rf";

        public int MaxOccurences => 1;
    }
}
