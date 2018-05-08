using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers
{
    public interface IDateTimeParserMulti : IHeaderParser<List<DateTime>> { }

    public class DateTimeParserMulti : HeaderParser<List<DateTime>>, IDateTimeParserMulti
    {
        private readonly IDateTimeConverter _converter;

        public DateTimeParserMulti(IDateTimeConverter converter)
        {
            _converter = converter;
        }

        protected override List<DateTime> DoConvert(List<string> values, string fieldName, bool valueMandatory, bool parseMandatory)
        {
            if (valueMandatory && !values.Any())
            {
                throw new ArgumentException($"Expected {fieldName} to have at least 1 value but had none.");
            }

            return values
                .Select(_ => _converter.Convert(_, fieldName, parseMandatory))
                .Where(_ => _ != null)
                .Select(_ => _.Value)
                .ToList();
        }

        protected override List<DateTime> OnFieldNotFoundReturnValue()
        {
            return new List<DateTime>();
        }
    }
}