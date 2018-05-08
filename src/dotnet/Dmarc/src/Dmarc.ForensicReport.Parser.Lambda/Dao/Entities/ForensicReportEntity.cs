using System;
using System.Collections.Generic;
using Dmarc.ForensicReport.Parser.Lambda.Domain;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class ForensicReportEntity
    {
        public long Id { get; set; }
        public string RequestId { get; set; }
        public string OrginalUri { get; set; }
        public DateTime CreatedDate { get; set; }
        public FeedbackType? FeedbackType { get; set; }
        public string UserAgent { get; set; }
        public string Version { get; set; }
        public AuthFailure? AuthFailure { get; set; }
        public string OriginalEnvelopeId { get; set; }
        public List<EmailAddressReportEntity> OriginalMailFroms { get; set; } = new List<EmailAddressReportEntity>();
        public DateTime? ArrivalDate { get; set; }
        public string ReportingMta { get; set; }
        public IpAddressEntity SourceIp { get; set; }
        public int? Incidents { get; set; }
        public DeliveryResult? DeliveryResult { get; set; }
        public string ProviderMessageId { get; set; }
        public string MessageId { get; set; }
        public string DkimDomain { get; set; }
        public string DkimIdentity { get; set; }
        public string DkimSelector { get; set; }
        public string DkimCanonicalizedHeader { get; set; }
        public string DkimCanonicalizedBody { get; set; }
        public string SpfDns { get; set; }
        public string AuthenticationResults { get; set; }
        public string ReportedDomain { get; set; }
        public List<EmailAddressReportEntity> OrginalRcptTos { get; set; } = new List<EmailAddressReportEntity>();
        public List<Rfc822HeaderSetEntity> Rfc822HeaderSets { get; set; } = new List<Rfc822HeaderSetEntity>();
        public List<ForensicBinaryEntity> BinaryMessageParts { get; set; } = new List<ForensicBinaryEntity>();
        public List<ForensicTextEntity> TextMessageParts { get; set; } = new List<ForensicTextEntity>();
        public List<ForensicReportUriEntity> ReportedUris { get; set; } = new List<ForensicReportUriEntity>();
    }


}
