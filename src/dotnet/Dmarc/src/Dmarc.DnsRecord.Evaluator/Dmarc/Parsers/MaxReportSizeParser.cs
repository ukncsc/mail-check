using System;
using System.Text.RegularExpressions;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public interface IMaxReportSizeParser
    {
        MaxReportSize Parse(string value);
    }

    public class MaxReportSizeParser : IMaxReportSizeParser
    {
        private readonly Regex _regex = new Regex(@"^(?<size>[0-9]+)(?<unit>[kmgt]{0,1})", RegexOptions.IgnoreCase);

        public MaxReportSize Parse(string value)
        {
            if (value != null)
            {
                Match match = _regex.Match(value);
                if (match.Success)
                {
                    string sizeToken = match.Groups["size"].Value.ToLower();
                    string unitToken = match.Groups["unit"].Value;

                    ulong candidateSize;
                    ulong? size = ulong.TryParse(sizeToken, out candidateSize)
                        ? candidateSize
                        : (ulong?) null;

                    Unit unit;
                    if (!Enum.TryParse(unitToken, true, out unit))
                    {
                        unit = Unit.B; //byte is default if no unit specified.
                    }

                    MaxReportSize maxReportSize = new MaxReportSize(size, unit);

                    if (!size.HasValue)
                    {
                        string maxSizeErrorMessage = string.Format(DmarcParserResource.InvalidValueErrorMessage,
                            "max size", sizeToken);
                        maxReportSize.AddError(new Error(ErrorType.Error, maxSizeErrorMessage));
                    }

                    if (!string.IsNullOrEmpty(unitToken) && unit == Unit.Unknown)
                    {
                        string unitErrorMessage = string.Format(DmarcParserResource.InvalidValueErrorMessage, "unit",
                            unitToken);
                        maxReportSize.AddError(new Error(ErrorType.Error, unitErrorMessage));
                    }
                    return maxReportSize;
                }
            }
            MaxReportSize invalidMaxReportSize = new MaxReportSize(null, Unit.Unknown);
            string maxReportSizeErrorMessage = string.Format(DmarcParserResource.InvalidValueErrorMessage, "max report size", value);
            invalidMaxReportSize.AddError(new Error(ErrorType.Error, maxReportSizeErrorMessage));
            return invalidMaxReportSize;
        }
    }
}