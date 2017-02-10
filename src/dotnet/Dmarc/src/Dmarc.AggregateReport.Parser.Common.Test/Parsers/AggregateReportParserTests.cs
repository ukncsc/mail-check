using System.Collections.Generic;
using System.IO;
using Dmarc.AggregateReport.Parser.Common.Converters;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Common.Email;
using Dmarc.AggregateReport.Parser.Common.Parsers;
using Dmarc.AggregateReport.Parser.Common.Persistence.Multi;
using Dmarc.AggregateReport.Parser.Common.Serialisation;
using Dmarc.AggregateReport.Parser.Common.Serialisation.AggregateReportDeserialisation;
using Dmarc.Common.Logging;
using FakeItEasy;
using MimeKit;
using NUnit.Framework;

namespace Dmarc.AggregateReport.Parser.Common.Test.Parsers
{
    [TestFixture]
    public class AggregateReportParserTests
    {
        private AggregateReportParserApp _aggregateReportParser;
        private IAttachmentStreamNormaliser _attachmentStreamNormaliser;
        private IAggregateReportDeserialiser _aggregateReportDeserialiser;
        private IDenormalisedRecordConverter _denormalisedRecordConverter;
        private IMultiAttachmentPersistor _multiAttachmentPersistor;
        private IMultiDenormalisedRecordPersistor _multiDenormalisedRecordPersistor;
        private IMultiAggregateReportPersistor _mulitAggregateReportPersistor;
        private ILogger _log;
        private IMimeMessageFactory _mimeMessageFactory;

        [SetUp]
        public void SetUp()
        {
            _mimeMessageFactory = A.Fake<IMimeMessageFactory>();
            _attachmentStreamNormaliser = A.Fake<IAttachmentStreamNormaliser>();
            _aggregateReportDeserialiser = A.Fake<IAggregateReportDeserialiser>();
            _denormalisedRecordConverter = A.Fake<IDenormalisedRecordConverter>();
            _multiAttachmentPersistor = A.Fake<IMultiAttachmentPersistor>();
            _multiDenormalisedRecordPersistor = A.Fake<IMultiDenormalisedRecordPersistor>();
            _mulitAggregateReportPersistor = A.Fake<IMultiAggregateReportPersistor>();
            _log = A.Fake<ILogger>();

            _aggregateReportParser = new AggregateReportParserApp(_mimeMessageFactory,
                _attachmentStreamNormaliser, _aggregateReportDeserialiser, _denormalisedRecordConverter, 
                _multiAttachmentPersistor, _multiDenormalisedRecordPersistor, _mulitAggregateReportPersistor, _log);
        }

        [Test]
        public void NoPersistorsActiveNoProcessingOccursUserIsWarned()
        {
            A.CallTo(() => _multiAttachmentPersistor.Active).Returns(false);
            A.CallTo(() => _multiDenormalisedRecordPersistor.Active).Returns(false);
            A.CallTo(() => _mulitAggregateReportPersistor.Active).Returns(false);
            A.CallTo(() => _mimeMessageFactory.Create(A<Stream>._)).Returns(CreateEmailMessage());

            _aggregateReportParser.Parse(new EmailMessageInfo(new EmailMetadata("", "", "", 100), Stream.Null));

            A.CallTo(() => _attachmentStreamNormaliser.Normalise(A<MimePart>._)).MustNotHaveHappened();
            A.CallTo(() => _aggregateReportDeserialiser.Deserialise(A<AttachmentInfo>._, A<EmailMetadata>._)).MustNotHaveHappened();
            A.CallTo(() => _log.Warn(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _denormalisedRecordConverter.ToDenormalisedRecord(A<Domain.Dmarc.AggregateReport>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _multiAttachmentPersistor.Persist(A<List<AttachmentInfo>>._)).MustNotHaveHappened();
            A.CallTo(() => _multiDenormalisedRecordPersistor.Persist(A<IEnumerable<DenormalisedRecord>>._)).MustNotHaveHappened();
            A.CallTo(() => _mulitAggregateReportPersistor.Persist(A<IEnumerable<AggregateReportInfo>>._)).MustNotHaveHappened();
        }

        [Test]
        public void AttachmentPersistorsActiveProcessingOccursAttachmentsPersisted()
        {
            A.CallTo(() => _multiAttachmentPersistor.Active).Returns(true);
            A.CallTo(() => _multiDenormalisedRecordPersistor.Active).Returns(false);
            A.CallTo(() => _mulitAggregateReportPersistor.Active).Returns(false);
            A.CallTo(() => _mimeMessageFactory.Create(A<Stream>._)).Returns(CreateEmailMessage());

            _aggregateReportParser.Parse(new EmailMessageInfo(new EmailMetadata("", "", "", 100), Stream.Null));

            A.CallTo(() => _attachmentStreamNormaliser.Normalise(A<MimePart>._)).MustHaveHappened();
            A.CallTo(() => _aggregateReportDeserialiser.Deserialise(A<AttachmentInfo>._, A<EmailMetadata>._)).MustNotHaveHappened();
            A.CallTo(() => _log.Warn(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _denormalisedRecordConverter.ToDenormalisedRecord(A<Domain.Dmarc.AggregateReport>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _multiAttachmentPersistor.Persist(A<List<AttachmentInfo>>._)).MustHaveHappened();
            A.CallTo(() => _multiDenormalisedRecordPersistor.Persist(A<IEnumerable<DenormalisedRecord>>._)).MustNotHaveHappened();
            A.CallTo(() => _mulitAggregateReportPersistor.Persist(A<IEnumerable<AggregateReportInfo>>._)).MustNotHaveHappened();
        }

        [Test]
        public void DenormalisedRecordPersistorsActiveProcessingOccursAggregateReportsPersisted()
        {
            A.CallTo(() => _multiAttachmentPersistor.Active).Returns(false);
            A.CallTo(() => _multiDenormalisedRecordPersistor.Active).Returns(true);
            A.CallTo(() => _mulitAggregateReportPersistor.Active).Returns(false);
            A.CallTo(() => _mimeMessageFactory.Create(A<Stream>._)).Returns(CreateEmailMessage());
            AggregateReportInfo aggregateReportInfo = new AggregateReportInfo(new Domain.Dmarc.AggregateReport(null, null, new Record[0]), null, new AttachmentMetadata(""));
            A.CallTo(() => _aggregateReportDeserialiser.Deserialise(A<AttachmentInfo>._, A<EmailMetadata>._)).ReturnsNextFromSequence(aggregateReportInfo);

            _aggregateReportParser.Parse(new EmailMessageInfo(new EmailMetadata("", "", "", 100), Stream.Null));

            A.CallTo(() => _attachmentStreamNormaliser.Normalise(A<MimePart>._)).MustHaveHappened();
            A.CallTo(() => _aggregateReportDeserialiser.Deserialise(A<AttachmentInfo>._, A<EmailMetadata>._)).MustHaveHappened();
            A.CallTo(() => _log.Warn(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _denormalisedRecordConverter.ToDenormalisedRecord(A<Domain.Dmarc.AggregateReport>._, A<string>._)).MustHaveHappened();
            A.CallTo(() => _multiAttachmentPersistor.Persist(A<List<AttachmentInfo>>._)).MustNotHaveHappened();
            A.CallTo(() => _multiDenormalisedRecordPersistor.Persist(A<IEnumerable<DenormalisedRecord>>._)).MustHaveHappened();
            A.CallTo(() => _mulitAggregateReportPersistor.Persist(A<IEnumerable<AggregateReportInfo>>._)).MustNotHaveHappened();
        }

        [Test]
        public void AggregateReportPersistorsActiveProcessingOccursAggregateReportsPersisted()
        {
            A.CallTo(() => _multiAttachmentPersistor.Active).Returns(false);
            A.CallTo(() => _multiDenormalisedRecordPersistor.Active).Returns(false);
            A.CallTo(() => _mulitAggregateReportPersistor.Active).Returns(true);
            A.CallTo(() => _mimeMessageFactory.Create(A<Stream>._)).Returns(CreateEmailMessage());
            AggregateReportInfo aggregateReportInfo = new AggregateReportInfo(new Domain.Dmarc.AggregateReport(null, null, new Record[0]), null, new AttachmentMetadata(""));
            A.CallTo(() => _aggregateReportDeserialiser.Deserialise(A<AttachmentInfo>._, A<EmailMetadata>._)).ReturnsNextFromSequence(aggregateReportInfo);

            _aggregateReportParser.Parse(new EmailMessageInfo(new EmailMetadata("", "", "", 100), Stream.Null));

            A.CallTo(() => _attachmentStreamNormaliser.Normalise(A<MimePart>._)).MustHaveHappened();
            A.CallTo(() => _aggregateReportDeserialiser.Deserialise(A<AttachmentInfo>._, A<EmailMetadata>._)).MustHaveHappened();
            A.CallTo(() => _log.Warn(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _denormalisedRecordConverter.ToDenormalisedRecord(A<Domain.Dmarc.AggregateReport>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _multiAttachmentPersistor.Persist(A<List<AttachmentInfo>>._)).MustNotHaveHappened();
            A.CallTo(() => _multiDenormalisedRecordPersistor.Persist(A<IEnumerable<DenormalisedRecord>>._)).MustNotHaveHappened();
            A.CallTo(() => _mulitAggregateReportPersistor.Persist(A<IEnumerable<AggregateReportInfo>>._)).MustHaveHappened();
        }

        private MimeMessage CreateEmailMessage()
        {
            var mimeMessage = new MimeMessage();

            var attachment = new MimePart("application", "gzip")
            {
                ContentObject = new ContentObject(new MemoryStream()),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = ""
            };

            var multipart = new Multipart("mixed") {attachment};

            mimeMessage.Body = multipart;

            return mimeMessage;
        }
    }
}
