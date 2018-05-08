using System.Collections.Generic;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common
{
    public interface IRawValueParserMulti : IHeaderParser<List<string>> { }

    public class RawValueParserMulti : HeaderParserMulti<string, List<string>>, IRawValueParserMulti
    {
        protected override string Convert(string value, string fieldName, bool parseMandatory)
        {
            return value;
        }
    }
}