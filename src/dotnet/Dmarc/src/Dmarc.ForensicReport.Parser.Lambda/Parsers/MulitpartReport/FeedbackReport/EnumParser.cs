using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport
{
    public abstract class EnumParser<T> : HeaderParserSingle<T?> where T : struct
    {
        private readonly IEnumConverter<T> _converter;

        protected EnumParser(IEnumConverter<T> converter)
        {
            _converter = converter;
        }

        protected override T? Convert(string value, string fieldName, bool parseMandatory)
        {
            return _converter.Convert(value, fieldName, parseMandatory);
        }
    }
}