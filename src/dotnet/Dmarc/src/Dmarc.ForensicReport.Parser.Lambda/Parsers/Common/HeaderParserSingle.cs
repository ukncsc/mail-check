using System.Collections.Generic;
using System.Linq;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common
{
    public abstract class HeaderParserSingle<T> : HeaderParser<T>
    {
        protected override T DoConvert(List<string> values, string fieldName, bool valueMandatory, bool parseMandatory)
        {
            string value = valueMandatory ? values.Single() : values.SingleOrDefault();

            return Convert(value, fieldName, parseMandatory);          
        }

        protected abstract T Convert(string value, string fieldName, bool parseMandatory);
    }
}