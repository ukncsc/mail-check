using System.Linq;
using System.Net.Mail;
using Dmarc.Common.Linq;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters
{
    public interface IMailAddressCollectionConverter : IConverter<MailAddressCollection> { }

    public class MailAddressCollectionConverter : IMailAddressCollectionConverter
    {
        private readonly IMailAddressConverter _mailAddressConverter;

        public MailAddressCollectionConverter(IMailAddressConverter mailAddressConverter)
        {
            _mailAddressConverter = mailAddressConverter;
        }

        public MailAddressCollection Convert(string value, string fieldName, bool parseMandatory)
        {
            InternetAddressList results;
            MailAddressCollection mailAddressCollection = new MailAddressCollection();
            if (InternetAddressList.TryParse(value, out results))
            {
                results.Mailboxes
                    .Select(_ => _mailAddressConverter.Convert(_.Address, fieldName, parseMandatory))
                    .Where(_ => _ != null)
                    .ForEach(mailAddressCollection.Add);
            }
            return mailAddressCollection;
        }
    }
}