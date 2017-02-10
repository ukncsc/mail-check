using System;
using System.IO;
using Dmarc.AggregateReport.Parser.Common.Compression;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Parsers;
using Dmarc.Common.Logging;
using FakeItEasy;
using MimeKit;
using NUnit.Framework;

namespace Dmarc.AggregateReport.Parser.Common.Test.Parsers
{
    [TestFixture]
    public class AttachmentStreamNormaliserTests
    {
        private AttachmentStreamNormaliser _attachmentStreamNormaliser;
        private IGZipDecompressor _gZipDecompressor;
        private IZipDecompressor _zipDecompressor;

        [SetUp]
        public void SetUp()
        {
            _gZipDecompressor = A.Fake<IGZipDecompressor>();
            _zipDecompressor = A.Fake<IZipDecompressor>();
            _attachmentStreamNormaliser = new AttachmentStreamNormaliser(_gZipDecompressor, _zipDecompressor, A.Fake<ILogger>());
        }

        [Test]
        public void ContentTypeApplicationGzipDecompressesAsGzip()
        {
            MimePart mimePart = CreateMimePart("application", "gzip", "test.gz");

            AttachmentInfo attachment = _attachmentStreamNormaliser.Normalise(mimePart);

            A.CallTo(() => _gZipDecompressor.Decompress(A<Stream>._)).MustHaveHappened(Repeated.Exactly.Once);

            Assert.That(attachment, Is.Not.Null);
            Assert.That(attachment.AttachmentMetadata.Filename, Is.EqualTo(mimePart.FileName));
        }

        [Test]
        public void ContentTypeApplicationZipDecompressesAsZip()
        {
            MimePart mimePart = CreateMimePart("application", "zip", "test.zip");

            AttachmentInfo attachment = _attachmentStreamNormaliser.Normalise(mimePart);

            A.CallTo(() => _zipDecompressor.Decompress(A<Stream>._)).MustHaveHappened(Repeated.Exactly.Once);

            Assert.That(attachment, Is.Not.Null);
            Assert.That(attachment.AttachmentMetadata.Filename, Is.EqualTo(mimePart.FileName));
        }

        [Test]
        public void ContentTypeApplicationOctectStreamWithGzFileExtensionDecompressesAsGzip()
        {
            MimePart mimePart = CreateMimePart("application", "octet-stream", "test.gz");

            AttachmentInfo attachment = _attachmentStreamNormaliser.Normalise(mimePart);

            A.CallTo(() => _gZipDecompressor.Decompress(A<Stream>._)).MustHaveHappened(Repeated.Exactly.Once);

            Assert.That(attachment, Is.Not.Null);
            Assert.That(attachment.AttachmentMetadata.Filename, Is.EqualTo(mimePart.FileName));
        }

        [Test]
        public void ContentTypeApplicationOctectStreamWithZipFileExtensionDecompressesAsZip()
        {
            MimePart mimePart = CreateMimePart("application", "octet-stream", "test.zip");

            AttachmentInfo attachment = _attachmentStreamNormaliser.Normalise(mimePart);

            A.CallTo(() => _zipDecompressor.Decompress(A<Stream>._)).MustHaveHappened(Repeated.Exactly.Once);

            Assert.That(attachment, Is.Not.Null);
            Assert.That(attachment.AttachmentMetadata.Filename, Is.EqualTo(mimePart.FileName));
        }

        [Test]
        public void ContentTypeApplicationOctectStreamWithNoExtensionDecompressesAsGzip()
        {
            MimePart mimePart = CreateMimePart("application", "octet-stream", "test");

            AttachmentInfo attachment = _attachmentStreamNormaliser.Normalise(mimePart);

            A.CallTo(() => _gZipDecompressor.Decompress(A<Stream>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _zipDecompressor.Decompress(A<Stream>._)).MustNotHaveHappened();

            Assert.That(attachment, Is.Not.Null);
            Assert.That(attachment.AttachmentMetadata.Filename, Is.EqualTo(mimePart.FileName));
        }

        [Test]
        public void ContentTypeApplicationOctectStreamWithNoExtensionDecompressesAsZipIfGzipFails()
        {
            MimePart mimePart = CreateMimePart("application", "octet-stream", "test");
            A.CallTo(() => _gZipDecompressor.Decompress(A<Stream>._)).Throws<Exception>();

            AttachmentInfo attachment = _attachmentStreamNormaliser.Normalise(mimePart);

            A.CallTo(() => _gZipDecompressor.Decompress(A<Stream>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _zipDecompressor.Decompress(A<Stream>._)).MustHaveHappened(Repeated.Exactly.Once);

            Assert.That(attachment, Is.Not.Null);
            Assert.That(attachment.AttachmentMetadata.Filename, Is.EqualTo(mimePart.FileName));
        }

        [Test]
        public void ContentTypeApplicationOctectStreamWithNoExtensionReturnsEmptyAttachmentIfGzipAndZipFails()
        {
            MimePart mimePart = CreateMimePart("application", "octet-stream", "test");
            A.CallTo(() => _gZipDecompressor.Decompress(A<Stream>._)).Throws<Exception>();
            A.CallTo(() => _zipDecompressor.Decompress(A<Stream>._)).Throws<Exception>();

            AttachmentInfo attachment = _attachmentStreamNormaliser.Normalise(mimePart);

            A.CallTo(() => _gZipDecompressor.Decompress(A<Stream>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _zipDecompressor.Decompress(A<Stream>._)).MustHaveHappened(Repeated.Exactly.Once);

            Assert.That(attachment, Is.Not.Null);
            Assert.That(attachment, Is.EqualTo(AttachmentInfo.EmptyAttachmentInfo));
        }

        [Test]
        public void ContentTypeApplicationXZipCompressedDecompressesAsZip()
        {
            MimePart mimePart = CreateMimePart("application", "x-zip-compressed", "test.zip");

            AttachmentInfo attachment = _attachmentStreamNormaliser.Normalise(mimePart);

            A.CallTo(() => _zipDecompressor.Decompress(A<Stream>._)).MustHaveHappened(Repeated.Exactly.Once);

            Assert.That(attachment, Is.Not.Null);
            Assert.That(attachment.AttachmentMetadata.Filename, Is.EqualTo(mimePart.FileName));
        }

        [Test]
        public void ContentTypeShouldntContainAggregateReportReturnsEmptyAttachment()
        {
            MimePart mimePart = CreateMimePart("text", "plain", "test.txt");

            AttachmentInfo attachment = _attachmentStreamNormaliser.Normalise(mimePart);

            Assert.That(attachment, Is.EqualTo(AttachmentInfo.EmptyAttachmentInfo));
        }

        [Test]
        public void ContentTypeDetectionIsNotCaseSensitive()
        {
            MimePart mimePart = CreateMimePart("AppLiCatIon", "GzIp", "test.gz");

            AttachmentInfo attachment = _attachmentStreamNormaliser.Normalise(mimePart);

            A.CallTo(() => _gZipDecompressor.Decompress(A<Stream>._)).MustHaveHappened(Repeated.Exactly.Once);

            Assert.That(attachment, Is.Not.Null);
            Assert.That(attachment.AttachmentMetadata.Filename, Is.EqualTo(mimePart.FileName));
        }


        private MimePart CreateMimePart(string mediaType, string mediaSubtype, string filename)
        {
            var contentType = new MimeKit.ContentType(mediaType, mediaSubtype);

            IContentObject contentObject = A.Fake<IContentObject>();
            A.CallTo(() => contentObject.Stream).Returns(new MemoryStream());

            return new MimePart(contentType)
            {
                FileName = filename,
                ContentObject = contentObject
            };
        }
    }
}
