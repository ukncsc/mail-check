using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport
{
    public interface IIntParser : IHeaderParser<int?> { }

    public class IntParser : HeaderParserSingle<int?>, IIntParser
    {
        private readonly IIntConverter _converter;

        public IntParser(IIntConverter converter)
        {
            _converter = converter;
        }

        protected override int? Convert(string value, string fieldName, bool parseMandatory)
        {
            return _converter.Convert(value, fieldName, parseMandatory);
        }
    }
}