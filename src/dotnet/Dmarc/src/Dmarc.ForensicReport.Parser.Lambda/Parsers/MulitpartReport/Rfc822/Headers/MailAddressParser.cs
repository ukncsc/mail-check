using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers
{
    public interface IMailAddressParser : IHeaderParser<MailAddress> { }

    public class MailAddressParser : HeaderParserSingle<MailAddress>, IMailAddressParser
    {
        private readonly IMailAddressConverter _converter;

        public MailAddressParser(IMailAddressConverter converter)
        {
            _converter = converter;
        }

        protected override MailAddress Convert(string value, string fieldName, bool parseMandatory)
        {
            return _converter.Convert(value, fieldName, parseMandatory);
        }
    }
}