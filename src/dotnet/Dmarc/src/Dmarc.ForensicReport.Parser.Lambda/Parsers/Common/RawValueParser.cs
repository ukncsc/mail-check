namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common
{
    public interface IRawValueParser : IHeaderParser<string> { }

    public class RawValueParser : HeaderParserSingle<string>, IRawValueParser
    {
        protected override string Convert(string value, string fieldName, bool parseMandatory)
        {
            return value;
        }
    }
}