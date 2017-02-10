using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Email;
using Dmarc.AggregateReport.Parser.Common.Parsers;
using Dmarc.AggregateReport.Parser.Common.Serialisation;
using Dmarc.AggregateReport.Parser.Common.Serialisation.AggregateReportDeserialisation;
using Dmarc.AggregateReport.Parser.Lambda.Converters;
using Dmarc.AggregateReport.Parser.Lambda.Dao;
using Dmarc.Common.Logging;
using MimeKit;

namespace Dmarc.AggregateReport.Parser.Lambda.Parser
{
    internal class AggregateReportParserLambda : IAggregateReportParserAsync
    {
        private readonly IMimeMessageFactory _mimeMessageFactory;
        private readonly IAttachmentStreamNormaliser _attachmentStreamNormaliser;
        private readonly IAggregateReportDeserialiser _aggregateReportDeserialiser;
        private readonly IAggregateReportToEntityConverter _aggregateReportToEntityConverter;
        private readonly IAggregateReportDao _aggregateReportDao;
        private readonly ILogger _log;

        public AggregateReportParserLambda(IMimeMessageFactory mimeMessageFactory,
            IAttachmentStreamNormaliser attachmentStreamNormaliser,
            IAggregateReportDeserialiser aggregateReportDeserialiser,
            IAggregateReportToEntityConverter aggregateReportToEntityConverter,
            IAggregateReportDao aggregateReportDao,
            ILogger log)
        {
            _mimeMessageFactory = mimeMessageFactory;
            _attachmentStreamNormaliser = attachmentStreamNormaliser;
            _aggregateReportDeserialiser = aggregateReportDeserialiser;
            _aggregateReportToEntityConverter = aggregateReportToEntityConverter;
            _aggregateReportDao = aggregateReportDao;
            _log = log;
        }

        public async Task Parse(EmailMessageInfo messageInfo)
        {
            _log.Debug($"Processing {messageInfo.EmailMetadata.OriginalUri}.");
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<AttachmentInfo> attachments = _mimeMessageFactory
                .Create(messageInfo.EmailStream)
                .BodyParts.OfType<MimePart>()
                .Select(_attachmentStreamNormaliser.Normalise)
                .Where(_ => !_.Equals(AttachmentInfo.EmptyAttachmentInfo))
                .ToList();

            if (attachments.Count == 0)
            {
                _log.Warn($"Didnt find any aggregate report attachments for {messageInfo.EmailMetadata.OriginalUri}.");
            }
            else
            {
                _log.Debug($"Found {attachments.Count} aggregate report attachment(s) for {messageInfo.EmailMetadata.OriginalUri}.");
            }
            _log.Debug($"Aggregate report attachment processing took {stopwatch.Elapsed}");
            stopwatch.Reset();

            stopwatch.Start();
            List<AggregateReportInfo> aggregateReports = attachments
                .Select(_ => _aggregateReportDeserialiser.Deserialise(_, messageInfo.EmailMetadata))
                .ToList();
            _log.Debug($"Deserialising aggregate reports took {stopwatch.Elapsed}");
            stopwatch.Stop();

            await Task.WhenAll(aggregateReports.Select(_ => _aggregateReportDao.Add(_aggregateReportToEntityConverter.ConvertToEntity(_))));

            attachments.ForEach(_ => _.Dispose());
        }
    }
}
