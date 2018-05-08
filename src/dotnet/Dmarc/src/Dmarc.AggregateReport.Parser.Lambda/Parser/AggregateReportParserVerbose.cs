using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Persistence.Multi;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Report.Email;
using Dmarc.Common.Report.Parser;
using MimeKit;

namespace Dmarc.AggregateReport.Parser.Lambda.Parser
{
    public class AggregateReportParserVerbose : IReportParser<AggregateReportInfo>
    {
        private readonly IMimeMessageFactory _mimeMessageFactory;
        private readonly IAttachmentStreamNormaliser _attachmentStreamNormaliser;
        private readonly IAggregateReportDeserialiser _aggregateReportDeserialiser;
        private readonly IMultiAttachmentPersistor _multiAttachmentPersistor;
        private readonly ILogger _log;

        public AggregateReportParserVerbose(IMimeMessageFactory mimeMessageFactory,
            IAttachmentStreamNormaliser attachmentStreamNormaliser,
            IAggregateReportDeserialiser aggregateReportDeserialiser,
            IMultiAttachmentPersistor multiAttachmentPersistor,
            ILogger log)
        {
            _mimeMessageFactory = mimeMessageFactory;
            _attachmentStreamNormaliser = attachmentStreamNormaliser;
            _aggregateReportDeserialiser = aggregateReportDeserialiser;
            _multiAttachmentPersistor = multiAttachmentPersistor;
            _log = log;
        }

        public AggregateReportInfo Parse(EmailMessageInfo messageInfo)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _log.Debug($"Processing {messageInfo.EmailMetadata.OriginalUri}.");
               
            List<AttachmentInfo> attachments = _mimeMessageFactory
                .Create(messageInfo.EmailStream)
                .BodyParts.OfType<MimePart>()
                .Select(_attachmentStreamNormaliser.Normalise)
                .Where(_ => !_.Equals(AttachmentInfo.EmptyAttachmentInfo))
                .ToList();

            _log.Debug($"Aggregate report attachment processing took {stopwatch.Elapsed}");
            stopwatch.Reset();

            AttachmentInfo attachment = attachments.Single();
                
            stopwatch.Start();
            AggregateReportInfo aggregateReport = _aggregateReportDeserialiser.Deserialise(attachment, messageInfo.EmailMetadata);
            _log.Debug($"Deserialising aggregate report took {stopwatch.Elapsed}");
            stopwatch.Reset();

            _log.Debug($"Found {aggregateReport.AggregateReport.Records.Length} records in {aggregateReport.AttachmentMetadata.Filename}");

            if (_multiAttachmentPersistor.Active)
            {
                stopwatch.Start();
                _multiAttachmentPersistor.Persist(attachment);
                _log.Debug($"Persisting attachments took {stopwatch.Elapsed}");
                stopwatch.Reset();
            }

            attachment?.Dispose();

            return aggregateReport;
        }
    }
}
