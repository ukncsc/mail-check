using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common
{
    public interface IMailAddressCollectionParser : IHeaderParser<MailAddressCollection> { }

    public class MailAddressCollectionParser : HeaderParserSingle<MailAddressCollection>, IMailAddressCollectionParser
    {
        private readonly IMailAddressCollectionConverter _converter;

        public MailAddressCollectionParser(IMailAddressCollectionConverter converter)
        {
            _converter = converter;
        }

        protected override MailAddressCollection Convert(string value, string fieldName, bool parseMandatory)
        {
            return _converter.Convert(value, fieldName, parseMandatory);
        }
    }
}