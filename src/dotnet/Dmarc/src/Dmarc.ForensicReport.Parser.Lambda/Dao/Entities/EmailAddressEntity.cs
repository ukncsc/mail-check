namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class EmailAddressEntity
    {
        public EmailAddressEntity(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public long Id { get; set; }
        public string EmailAddress { get; }
    }
}
