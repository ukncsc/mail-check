using System;
using System.IO;
using System.Text;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using Dmarc.Common.Report.Domain;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    [TestFixture]
    public class AggregateReportDeserialiserTests
    {
        private AggregateReportDeserialiser _aggregateReportDeserialiser;
        private IReportMetadataDeserialiser _reportMetadataDeserialiser;
        private IPolicyPublishedDeserialiser _policyPublishedDeserialiser;
        private IRecordDeserialiser _recordDeserialiser;

        [SetUp]
        public void SetUp()
        {
            _reportMetadataDeserialiser = A.Fake<IReportMetadataDeserialiser>();
            _policyPublishedDeserialiser = A.Fake<IPolicyPublishedDeserialiser>();
            _recordDeserialiser = A.Fake<IRecordDeserialiser>();
            _aggregateReportDeserialiser = new AggregateReportDeserialiser(
                _reportMetadataDeserialiser, 
                _policyPublishedDeserialiser, 
                _recordDeserialiser);
        }

        [Test]
        public void RootMustBeFeedback()
        {
            AttachmentInfo attachmentInfo = CreateAttachmentInfo(AggregateReportDeserialiserTestsResource.RootNotFeedback);
            EmailMetadata emailMetadata = CreateEmailMetadata();

            Assert.Throws<ArgumentException>(() => _aggregateReportDeserialiser.Deserialise(attachmentInfo, emailMetadata));
        }

        [Test]
        public void CorrectlyFormedReportGeneratesAggregateReportInfo()
        {
            AttachmentInfo attachmentInfo = CreateAttachmentInfo(AggregateReportDeserialiserTestsResource.CorrectlyFormedReport);
            EmailMetadata emailMetadata = CreateEmailMetadata();

            AggregateReportInfo aggregateReportInfo = _aggregateReportDeserialiser.Deserialise(attachmentInfo, emailMetadata);
            Assert.That(aggregateReportInfo, Is.Not.Null);
        }

        [Test]
        public void CorrectlyFormedReportNoDeclarationGeneratesAggregateReportInfo()
        {
            AttachmentInfo attachmentInfo = CreateAttachmentInfo(AggregateReportDeserialiserTestsResource.CorrectlyFormedReportNoDeclaration);
            EmailMetadata emailMetadata = CreateEmailMetadata();

            AggregateReportInfo aggregateReportInfo = _aggregateReportDeserialiser.Deserialise(attachmentInfo, emailMetadata);
            Assert.That(aggregateReportInfo, Is.Not.Null);
        }

        [Test]
        public void ReportMetadataTagsMustExist()
        {
            AttachmentInfo attachmentInfo = CreateAttachmentInfo(AggregateReportDeserialiserTestsResource.NoReportMetadataTags);
            EmailMetadata emailMetadata = CreateEmailMetadata();

            Assert.Throws<InvalidOperationException>(() => _aggregateReportDeserialiser.Deserialise(attachmentInfo, emailMetadata));
        }

        [Test]
        public void PolicyPublishedTagsMustExist()
        {
            AttachmentInfo attachmentInfo = CreateAttachmentInfo(AggregateReportDeserialiserTestsResource.NoPolicyPublishedTags);
            EmailMetadata emailMetadata = CreateEmailMetadata();

            Assert.Throws<InvalidOperationException>(() => _aggregateReportDeserialiser.Deserialise(attachmentInfo, emailMetadata));
        }

        [Test]
        public void RecordTagsMustExist()
        {
            AttachmentInfo attachmentInfo = CreateAttachmentInfo(AggregateReportDeserialiserTestsResource.NoRecordTags);
            EmailMetadata emailMetadata = CreateEmailMetadata();

            Assert.Throws<ArgumentException>(() => _aggregateReportDeserialiser.Deserialise(attachmentInfo, emailMetadata));
        }

        [Test]
        public void ReportMetadataTagsMustNotOccurMoreThanOnce()
        {
            AttachmentInfo attachmentInfo = CreateAttachmentInfo(AggregateReportDeserialiserTestsResource.MultipleReportMetadataTags);
            EmailMetadata emailMetadata = CreateEmailMetadata();

            Assert.Throws<InvalidOperationException>(() => _aggregateReportDeserialiser.Deserialise(attachmentInfo, emailMetadata));
        }

        [Test]
        public void PolicyPublishedTagsMustNotOccurMoreThanOnce()
        {
            AttachmentInfo attachmentInfo = CreateAttachmentInfo(AggregateReportDeserialiserTestsResource.MultiplePolicyPublishedTags);
            EmailMetadata emailMetadata = CreateEmailMetadata();

            Assert.Throws<InvalidOperationException>(() => _aggregateReportDeserialiser.Deserialise(attachmentInfo, emailMetadata));
        }

        [Test]
        public void RecordTagsCanOccurrMultipleTimes()
        {
            AttachmentInfo attachmentInfo = CreateAttachmentInfo(AggregateReportDeserialiserTestsResource.CorrectlyFormedReportNoDeclaration);
            EmailMetadata emailMetadata = CreateEmailMetadata();

            AggregateReportInfo aggregateReportInfo = _aggregateReportDeserialiser.Deserialise(attachmentInfo, emailMetadata);
            Assert.That(aggregateReportInfo, Is.Not.Null);
        }

        [Test]
        public void TagsMustBeDirectDecendants()
        {
            AttachmentInfo attachmentInfo = CreateAttachmentInfo(AggregateReportDeserialiserTestsResource.TagsNotDirectDescendants);
            EmailMetadata emailMetadata = CreateEmailMetadata();

            Assert.Throws<InvalidOperationException>(() => _aggregateReportDeserialiser.Deserialise(attachmentInfo, emailMetadata));
        }

        private AttachmentInfo CreateAttachmentInfo(string data)
        {
            AttachmentMetadata attachmentMetadata = new AttachmentMetadata("filename");
            Stream stream =  new MemoryStream(Encoding.UTF8.GetBytes(data));

            return new AttachmentInfo(attachmentMetadata, stream);
        }

        private EmailMetadata CreateEmailMetadata()
        {
            return new EmailMetadata("requestId", "messageId", "orginalUri", "filename", 10);
        }
    }
}
