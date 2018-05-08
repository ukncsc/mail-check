using System;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class Rfc822HeaderEntity
    {
        public Rfc822HeaderEntity(
            Rfc822HeaderFieldEntity headerField, 
            int order, 
            Rfc822HeaderTextValueEntity textValue, 
            EmailAddressEntity emailAddress, 
            IpAddressEntity ipAddress,
            DateTime? date, 
            HostnameEntity hostname)
        {
            HeaderField = headerField;
            Order = order;
            TextValue = textValue;
            EmailAddress = emailAddress;
            IpAddress = ipAddress;
            Date = date;
            Hostname = hostname;
        }

        public long Id { get; set; }
        public long HeaderSetId { get; set; }
        public Rfc822HeaderFieldEntity HeaderField { get; set; }
        public Rfc822HeaderTextValueEntity TextValue { get; set; }
        public EmailAddressEntity EmailAddress { get; set; }
        public IpAddressEntity IpAddress { get; set; }
        public HostnameEntity Hostname { get; set; }
        public int Order { get; }
        public DateTime? Date { get; }
    }
}
