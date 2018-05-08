using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public class FeedbackReport : EmailPart
    {
        public FeedbackReport(string contentType, int depth, Disposition disposition) 
            : base(contentType, depth, disposition)
        {
        }

        //required single
        //rfc5965
        public FeedbackType? FeedbackType { get; set; }
        //rfc5965
        public string UserAgent { get; set; }
        //rfc5965
        public string Version { get; set; }
        //rfc6591
        public AuthFailure? AuthFailure { get; set; }
        //optional single
        //rfc5965
        public string OriginalEnvelopeId { get; set; }
        //rfc5965
        //If we dont have this then we can use rfc822 From field
        public MailAddressCollection OriginalMailFrom { get; set; }
        //rfc5965
        //If arrival date not populated but received date is then we will use that as received date.
        public DateTime? ArrivalDate { get; set; }
        //rfc5965
        public string ReportingMta { get; set; }
        //rfc5965
        public IPAddress SourceIp { get; set; }
        //rfc5965
        public int? Indicents { get; set; }
        //rfc6591
        public DeliveryResult? DeliveryResult { get; set; }
        public MailAddress MessageId { get; set; }
        //required for dkim reports
        //rfc6591
        public string DkimDomain { get; set; }
        //rfc6591
        public string DkimIdentity { get; set; }
        //rfc6591
        public string DkimSelector { get; set; }
        //optional for dkim reports
        //rfc6591
        public string DkimCanonicalizedHeader { get; set; }
        //rfc6591
        public string DkimCanonicalizedBody { get; set; }
        //required for spf reports
        //rfc6591
        public string SpfDns { get; set; }
        //rfc5965
        //If if dont have these from rfc5965 then we can try to get it from rfc822 AuthenticationResults
        public string AuthenticationResults { get; set; }
        //rfc6591
        public string ReportedDomain { get; set; }
        //optional multiple
        //rfc5965
        //If dont get these from rfc5965 then we can try to get from rfc822 To field
        public List<MailAddress> OrginalRcptTos { get; set; } = new List<MailAddress>();
        //rfc5965
        public List<string> ReportedUris { get; set; } = new List<string>();
    }
}