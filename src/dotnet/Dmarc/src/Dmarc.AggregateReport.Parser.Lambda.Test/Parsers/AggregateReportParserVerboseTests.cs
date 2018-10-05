using System.IO;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Parser;
using Dmarc.AggregateReport.Parser.Lambda.Persistence.Multi;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Report.Email;
using FakeItEasy;
using MimeKit;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Parsers
{
    [TestFixture]
    public class AggregateReportParserVerboseTests
    {
        private AggregateReportParserVerbose _aggregateReportParser;
        private IAttachmentStreamNormaliser _attachmentStreamNormaliser;
        private IAggregateReportDeserialiser _aggregateReportDeserialiser;
        private IMultiAttachmentPersistor _multiAttachmentPersistor;
        private ILogger _log;
        private IMimeMessageFactory _mimeMessageFactory;

        [SetUp]
        public void SetUp()
        {
            _mimeMessageFactory = A.Fake<IMimeMessageFactory>();
            _attachmentStreamNormaliser = A.Fake<IAttachmentStreamNormaliser>();
            _aggregateReportDeserialiser = A.Fake<IAggregateReportDeserialiser>();
            _multiAttachmentPersistor = A.Fake<IMultiAttachmentPersistor>();
            _log = A.Fake<ILogger>();

            _aggregateReportParser = new AggregateReportParserVerbose(_mimeMessageFactory,
                _attachmentStreamNormaliser, 
                _aggregateReportDeserialiser, 
                _multiAttachmentPersistor, 
                _log);
        }

        [Test]
        public void AggregateReportInfoReturned()
        {
            A.CallTo(() => _mimeMessageFactory.Create(A<Stream>._)).Returns(CreateEmailMessage());
            A.CallTo(() => _aggregateReportDeserialiser.Deserialise(A<AttachmentInfo>._, A<EmailMetadata>._)).Returns(CreateAggregateReportInfo());

            AggregateReportInfo aggregateReportInfo = _aggregateReportParser.Parse(new EmailMessageInfo(new EmailMetadata("", "", "", "", 100), Stream.Null));

            Assert.That(aggregateReportInfo, Is.Not.Null);
        }

        [Test]
        public void MultiAttachmentPersistorActiveAttachmentsPersisted()
        {
            A.CallTo(() => _mimeMessageFactory.Create(A<Stream>._)).Returns(CreateEmailMessage());
            A.CallTo(() => _aggregateReportDeserialiser.Deserialise(A<AttachmentInfo>._, A<EmailMetadata>._)).Returns(CreateAggregateReportInfo());
            A.CallTo(() => _multiAttachmentPersistor.Active).Returns(true);

            _aggregateReportParser.Parse(new EmailMessageInfo(new EmailMetadata("", "", "", "", 100), Stream.Null));

            A.CallTo(() => _multiAttachmentPersistor.Persist(A<AttachmentInfo>._)) .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void MultiAttachmentPersistorNotActiveAttachmentsNotPersisted()
        {
            A.CallTo(() => _mimeMessageFactory.Create(A<Stream>._)).Returns(CreateEmailMessage());
            A.CallTo(() => _aggregateReportDeserialiser.Deserialise(A<AttachmentInfo>._, A<EmailMetadata>._)).Returns(CreateAggregateReportInfo());
            A.CallTo(() => _multiAttachmentPersistor.Active).Returns(false);

            _aggregateReportParser.Parse(new EmailMessageInfo(new EmailMetadata("", "", "", "", 100), Stream.Null));

            A.CallTo(() => _multiAttachmentPersistor.Persist(A<AttachmentInfo>._)).MustNotHaveHappened();
        }

        private MimeMessage CreateEmailMessage()
        {
            var mimeMessage = new MimeMessage();

            var attachment = new MimePart("application", "gzip")
            {
                Content = new MimeContent(new MemoryStream()),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = ""
            };

            var multipart = new Multipart("mixed") {attachment};

            mimeMessage.Body = multipart;

            return mimeMessage;
        }

        private AggregateReportInfo CreateAggregateReportInfo()
        {
            Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.AggregateReport aggregateReport = new Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.AggregateReport {Records = new Record[0]};
            EmailMetadata emailMetadata = new EmailMetadata("", "", 1);
            AttachmentMetadata attachmentMetadata = new AttachmentMetadata("");
            return new AggregateReportInfo(aggregateReport, emailMetadata, attachmentMetadata);
        }
    }
}
