using System.Collections.Generic;
using System.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Report.Email;
using Dmarc.Common.Report.Parser;
using MimeKit;

namespace Dmarc.AggregateReport.Parser.Lambda.Parser
{
    internal class AggregateReportParser : IReportParser<AggregateReportInfo>
    {
        private readonly IMimeMessageFactory _mimeMessageFactory;
        private readonly IAttachmentStreamNormaliser _attachmentStreamNormaliser;
        private readonly IAggregateReportDeserialiser _aggregateReportDeserialiser;
        private readonly ILogger _log;

        public AggregateReportParser(IMimeMessageFactory mimeMessageFactory,
            IAttachmentStreamNormaliser attachmentStreamNormaliser,
            IAggregateReportDeserialiser aggregateReportDeserialiser,
            ILogger log)
        {
            _mimeMessageFactory = mimeMessageFactory;
            _attachmentStreamNormaliser = attachmentStreamNormaliser;
            _aggregateReportDeserialiser = aggregateReportDeserialiser;
            _log = log;
        }

        public AggregateReportInfo Parse(EmailMessageInfo messageInfo)
        {
            _log.Info($"Processing attachments for {messageInfo.EmailMetadata.OriginalUri}, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}.");
            List<AttachmentInfo> attachments = _mimeMessageFactory
                .Create(messageInfo.EmailStream)
                .BodyParts.OfType<MimePart>()
                .Select(_attachmentStreamNormaliser.Normalise)
                .Where(_ => !_.Equals(AttachmentInfo.EmptyAttachmentInfo))
                .ToList();

            _log.Info($"Successfully processed attachments for {messageInfo.EmailMetadata.OriginalUri}, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}.");

            AttachmentInfo attachment = attachments.Single();

            _log.Debug($"Deserialising aggregate report {attachment.AttachmentMetadata.Filename} for {messageInfo.EmailMetadata.OriginalUri}, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}.");
            AggregateReportInfo aggregateReportInfo = _aggregateReportDeserialiser.Deserialise(attachment, messageInfo.EmailMetadata);
            _log.Debug($"Successfully deserialised aggregate report with Id: {aggregateReportInfo.AggregateReport.ReportMetadata.ReportId} from file {attachment.AttachmentMetadata.Filename} from S3 bucket {messageInfo.EmailMetadata.OriginalUri}, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}.");

            attachments.ForEach(_ => _.Dispose());

            return aggregateReportInfo;
        }
    }
}