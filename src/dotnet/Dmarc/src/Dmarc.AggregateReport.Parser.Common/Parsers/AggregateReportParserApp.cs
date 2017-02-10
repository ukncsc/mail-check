using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dmarc.AggregateReport.Parser.Common.Converters;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Email;
using Dmarc.AggregateReport.Parser.Common.Persistence.Multi;
using Dmarc.AggregateReport.Parser.Common.Serialisation;
using Dmarc.AggregateReport.Parser.Common.Serialisation.AggregateReportDeserialisation;
using Dmarc.Common.Logging;
using MimeKit;

namespace Dmarc.AggregateReport.Parser.Common.Parsers
{
    public class AggregateReportParserApp : IAggregateReportParser, IDisposable
    {
        private readonly IMimeMessageFactory _mimeMessageFactory;
        private readonly IAttachmentStreamNormaliser _attachmentStreamNormaliser;
        private readonly IAggregateReportDeserialiser _aggregateReportDeserialiser;
        private readonly IDenormalisedRecordConverter _denormalisedRecordConverter;
        private readonly IMultiAttachmentPersistor _multiAttachmentPersistor;
        private readonly IMultiDenormalisedRecordPersistor _multiDenormalisedRecordPersistor;
        private readonly IMultiAggregateReportPersistor _multiAggregateReportPersistor;
        private readonly ILogger _log;

        public AggregateReportParserApp(IMimeMessageFactory mimeMessageFactory,
            IAttachmentStreamNormaliser attachmentStreamNormaliser,
            IAggregateReportDeserialiser aggregateReportDeserialiser,
            IDenormalisedRecordConverter denormalisedRecordConverter,
            IMultiAttachmentPersistor multiAttachmentPersistor,
            IMultiDenormalisedRecordPersistor multiDenormalisedRecordPersistor,
            IMultiAggregateReportPersistor multiAggregateReportPersistor,
            ILogger log)
        {
            _mimeMessageFactory = mimeMessageFactory;
            _attachmentStreamNormaliser = attachmentStreamNormaliser;
            _aggregateReportDeserialiser = aggregateReportDeserialiser;
            _denormalisedRecordConverter = denormalisedRecordConverter;
            _multiAttachmentPersistor = multiAttachmentPersistor;
            _multiDenormalisedRecordPersistor = multiDenormalisedRecordPersistor;
            _multiAggregateReportPersistor = multiAggregateReportPersistor;
            _log = log;
        }

        public void Parse(EmailMessageInfo messageInfo)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                List<AttachmentInfo> attachments = null;
                if (_multiAttachmentPersistor.Active || 
                    _multiDenormalisedRecordPersistor.Active ||
                    _multiAggregateReportPersistor.Active)
                {
                    _log.Debug($"Processing {messageInfo.EmailMetadata.OriginalUri}.");
                    stopwatch.Start();
                    
                    attachments = _mimeMessageFactory
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
                }
                else
                {
                    _log.Warn($"Not Processing {messageInfo.EmailMetadata.OriginalUri} as no persistors found.");
                }

                List<AggregateReportInfo> aggregateReports = null;
                if (_multiDenormalisedRecordPersistor.Active
                     || _multiAggregateReportPersistor.Active)
                {
                    stopwatch.Start();
                    aggregateReports = attachments
                        .Select(_ => _aggregateReportDeserialiser.Deserialise(_, messageInfo.EmailMetadata))
                        .ToList();
                    _log.Debug($"Deserialising aggregate reports took {stopwatch.Elapsed}");
                    stopwatch.Reset();

                    aggregateReports.ForEach(_ => _log.Debug($"Found {_.AggregateReport.Records.Length} records in {_.AttachmentMetadata.Filename}"));
                }

                List<DenormalisedRecord> denormalisedRecords = null;
                if (_multiDenormalisedRecordPersistor.Active)
                {
                    stopwatch.Start();
                    denormalisedRecords = aggregateReports
                        .Select(_ => _denormalisedRecordConverter.ToDenormalisedRecord(_.AggregateReport, messageInfo.EmailMetadata.OriginalUri))
                        .SelectMany(_ => _)
                        .ToList();
                    _log.Debug($"Denormalised record processing took {stopwatch.Elapsed}");
                    stopwatch.Reset();
                }

                if (_multiAttachmentPersistor.Active)
                {
                    stopwatch.Start();
                    _multiAttachmentPersistor.Persist(attachments);
                    _log.Debug($"Persisting attachments took {stopwatch.Elapsed}");
                    stopwatch.Reset();
                }

                if (_multiDenormalisedRecordPersistor.Active)
                {
                    stopwatch.Start();
                    _multiDenormalisedRecordPersistor.Persist(denormalisedRecords);
                    _log.Debug($"Persisting denormalised records took {stopwatch.Elapsed}");
                    stopwatch.Reset();
                }

                if (_multiAggregateReportPersistor.Active)
                {
                    stopwatch.Start();
                    _multiAggregateReportPersistor.Persist(aggregateReports);
                    _log.Debug($"Persisting denormalised records took {stopwatch.Elapsed}");
                    stopwatch.Reset();
                }

                attachments?.ForEach(_ => _.Dispose());
            }
            catch (Exception e)
            {
                _log.Error($"An error occurred processing {messageInfo.EmailMetadata.OriginalUri}{Environment.NewLine}{e.Message}{Environment.NewLine}{Environment.NewLine}{e.StackTrace}");
            }
        }

        public void Dispose()
        {
            (_multiDenormalisedRecordPersistor as IDisposable)?.Dispose();
        }
    }
}
