using System;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public interface IUriTagParser
    {
        UriTag Parse(string uriString);
    }

    public class UriTagParser : IUriTagParser
    {
        private const char Separator = '!';

        private readonly IDmarcUriParser _dmarcUriParser;
        private readonly IMaxReportSizeParser _maxReportSizeParser;

        public UriTagParser(IDmarcUriParser dmarcUriParser,
            IMaxReportSizeParser maxReportSizeParser)
        {
            _dmarcUriParser = dmarcUriParser;
            _maxReportSizeParser = maxReportSizeParser;
        }

        public UriTag Parse(string uriString)
        {
            string[] tokens = uriString?.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).ToArray() ?? new string[0];

            DmarcUri dmarcUri = _dmarcUriParser.Parse(tokens.ElementAtOrDefault(0));

            MaxReportSize maxReportSize = tokens.Length > 1
                ? _maxReportSizeParser.Parse(tokens[1])
                : null;

            UriTag uriTag = new UriTag(uriString, dmarcUri, maxReportSize);

            if (tokens.Length > 2)
            {
                string unexpectedValues = string.Join(",", tokens.Skip(2));
                string unexpectedValuesErrorMessage = string.Format(DmarcParserResource.UnexpectedValueErrorMessage, unexpectedValues, "uri", unexpectedValues);
                uriTag.AddError(new Error(ErrorType.Error, unexpectedValuesErrorMessage));
            }

            return uriTag;
        }
    }
}