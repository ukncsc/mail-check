using System;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common
{
    public interface IDateTimeParser : IHeaderParser<DateTime?> { }

    public class DateTimeParser : HeaderParserSingle<DateTime?>, IDateTimeParser
    {
        private readonly IDateTimeConverter _converter;

        public DateTimeParser(IDateTimeConverter converter)
        {
            _converter = converter;
        }

        protected override DateTime? Convert(string value, string fieldName, bool parseMandatory)
        {
            return _converter.Convert(value, fieldName, parseMandatory);
        }
    }
}