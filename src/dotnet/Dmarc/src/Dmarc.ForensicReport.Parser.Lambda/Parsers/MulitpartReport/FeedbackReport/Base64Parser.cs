using System.Text.RegularExpressions;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport
{
    public interface IBase64Parser : IHeaderParser<string> { }

    public class Base64Parser : HeaderParserSingle<string>, IBase64Parser
    {
        protected override string Convert(string value, string fieldName, bool parseMandatory)
        {
            return value != null ? Regex.Replace(value, @"\s+", string.Empty) : null;
        }
    }
}