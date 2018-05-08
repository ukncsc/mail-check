using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Report.Conversion;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Domain;

namespace Dmarc.ForensicReport.Parser.Lambda.Converters
{
    public class ForensicReportToEntityConverter : IToEntityConverter<ForensicReportInfo, ForensicReportEntity>
    {
        private readonly IIpAddressToEntityConverter _ipAddressToEntityConverter;
        private readonly IForensicReportEmailAddressToEntityConverter _forensicReportEmailAddressToEntityConverter;
        private readonly IRfc822ToEntityConverter _rfc822ToEntityConverter;
        private readonly IMimeContentConverter _mimeContentConverter;
        private readonly ITextContentToEntityConverter _textContentToEntityConverter;
        private readonly IForensicReportUriToEntityConverter _forensicReportUriToEntityConverter;

        public ForensicReportToEntityConverter(IIpAddressToEntityConverter ipAddressToEntityConverter,
            IForensicReportEmailAddressToEntityConverter forensicReportEmailAddressToEntityConverter,
            IRfc822ToEntityConverter rfc822ToEntityConverter,
            IMimeContentConverter mimeContentConverter,
            ITextContentToEntityConverter textContentToEntityConverter,
            IForensicReportUriToEntityConverter forensicReportUriToEntityConverter)
        {
            _ipAddressToEntityConverter = ipAddressToEntityConverter;
            _forensicReportEmailAddressToEntityConverter = forensicReportEmailAddressToEntityConverter;
            _rfc822ToEntityConverter = rfc822ToEntityConverter;
            _mimeContentConverter = mimeContentConverter;
            _textContentToEntityConverter = textContentToEntityConverter;
            _forensicReportUriToEntityConverter = forensicReportUriToEntityConverter;
        }

        public ForensicReportEntity Convert(ForensicReportInfo forensicReportInfo)
        {
            IEnumerable<FeedbackReport> feedbackReports = forensicReportInfo.ForensicReport.EmailParts.OfType<FeedbackReport>();
            FeedbackReport feedbackReport = feedbackReports.Single();

            return new ForensicReportEntity
            {
                RequestId = forensicReportInfo.EmailMetadata.RequestId,
                OrginalUri = forensicReportInfo.EmailMetadata.OriginalUri,
                CreatedDate = DateTime.UtcNow,
                FeedbackType = feedbackReport.FeedbackType,
                UserAgent = feedbackReport.UserAgent,
                Version = feedbackReport.Version,
                AuthFailure = feedbackReport.AuthFailure,
                OriginalEnvelopeId = feedbackReport.OriginalEnvelopeId,
                OriginalMailFroms = feedbackReport.OriginalMailFrom?.Select(_forensicReportEmailAddressToEntityConverter.Convert).ToList(),
                ArrivalDate = feedbackReport.ArrivalDate,
                ReportingMta = feedbackReport.ReportingMta,
                SourceIp = _ipAddressToEntityConverter.Convert(feedbackReport.SourceIp),
                Incidents = feedbackReport.Indicents,
                DeliveryResult = feedbackReport.DeliveryResult,
                MessageId = feedbackReport.MessageId?.Address,
                ProviderMessageId = forensicReportInfo.ForensicReport.ProviderMessageId,
                DkimDomain =  feedbackReport.DkimDomain,
                DkimIdentity = feedbackReport.DkimIdentity,
                DkimSelector = feedbackReport.DkimSelector,
                DkimCanonicalizedHeader = feedbackReport.DkimCanonicalizedHeader,
                DkimCanonicalizedBody = feedbackReport.DkimCanonicalizedBody,
                SpfDns = feedbackReport.SpfDns,
                AuthenticationResults =  feedbackReport.AuthenticationResults,
                ReportedDomain = feedbackReport.ReportedDomain,
                OrginalRcptTos = feedbackReport.OrginalRcptTos.Select(_forensicReportEmailAddressToEntityConverter.Convert).ToList(),
                Rfc822HeaderSets = forensicReportInfo.ForensicReport.EmailParts.OfType<Rfc822>().Select((value, index) => _rfc822ToEntityConverter.Convert(value, index)).ToList(),
                BinaryMessageParts = forensicReportInfo.ForensicReport.EmailParts.OfType<MimeContent>().Select(_mimeContentConverter.Convert).ToList(),
                TextMessageParts = forensicReportInfo.ForensicReport.EmailParts.OfType<TextContent>().Select(_textContentToEntityConverter.Convert).ToList(),
                ReportedUris = feedbackReport.ReportedUris.Select(_forensicReportUriToEntityConverter.Convert).ToList()
            };
        }
    }
}
