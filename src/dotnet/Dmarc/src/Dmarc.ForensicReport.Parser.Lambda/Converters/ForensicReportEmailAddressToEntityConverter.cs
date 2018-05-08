using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Converters
{
    public interface IForensicReportEmailAddressToEntityConverter
    {
        EmailAddressReportEntity Convert(MailAddress mailAddress);
    }

    public class ForensicReportEmailAddressToEntityConverter : IForensicReportEmailAddressToEntityConverter
    {
        public EmailAddressReportEntity Convert(MailAddress mailAddress)
        {
            return new EmailAddressReportEntity(new EmailAddressEntity(mailAddress.Address));
        }
    }
}
