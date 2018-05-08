namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class EmailAddressReportEntity
    {
        public EmailAddressReportEntity(EmailAddressEntity emailAddressEntity)
        {
            EmailAddressEntity = emailAddressEntity;
        }

        public long ReportId { get; set; }
        public EmailAddressEntity EmailAddressEntity { get; set; }
    }
}
