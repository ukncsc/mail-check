using System.Collections.Generic;
using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common
{
    public interface IMailAddressParserMulti : IHeaderParser<List<MailAddress>>{}

    public class MailAddressParserMulti : HeaderParserMulti<MailAddress, List<MailAddress>>, IMailAddressParserMulti
    {
        private readonly IMailAddressConverter _converter;

        public MailAddressParserMulti(IMailAddressConverter converter)
        {
            _converter = converter;
        }

        protected override MailAddress Convert(string value, string fieldName, bool parseMandatory)
        {
            return _converter.Convert(value, fieldName, parseMandatory);
        }
    }
}