using System.IO;
using System.Text;
using Dmarc.AggregateReport.Parser.Lambda.Parser;
using Dmarc.Common.Interface.Logging;
using FakeItEasy;
using MimeKit;
using NUnit.Framework;
using ContentType = Dmarc.AggregateReport.Parser.Lambda.Parser.ContentType;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Parsers
{
    [TestFixture]
    public class ContentTypeProviderTests
    {
        private ContentTypeProvider _contentTypeProvider;

        [SetUp]
        public void SetUp()
        {
            _contentTypeProvider = new ContentTypeProvider(A.Fake<ILogger>());
        }

        [Test]
        public void ContentTypeOfTextPlainWithoutAlternativeContentTypeHeaderReturnsPlainText()
        {
            MimePart mimePart = CreateMimePart("Text", "Plain", "test.gz");

            string contentType = _contentTypeProvider.GetContentType(mimePart);

            Assert.That(contentType, Is.EqualTo(ContentType.TextPlain));
        }

        [Test]
        public void ContentTypeOfTextPlainWithAlternativeContentTypeReturnAlternativeContentType()
        {
            MimePart mimePart = CreateMimePart("Text", "Plain", "test.gz",
                new Header(Encoding.UTF8, "ContentType", "application/gzip;	name=\"embltd.co.uk!lincolnshire.gov.uk!1520846256!1520935058!827.xml.gz\""));

            string contentType = _contentTypeProvider.GetContentType(mimePart);

            Assert.That(contentType, Is.EqualTo(ContentType.ApplicationGzip));
        }

        [Test]
        public void ContentTypeOfNotTextPlainReturnsOriginalContentType()
        {
            MimePart mimePart = CreateMimePart("application", "octet-stream", "test.gz",
                new Header(Encoding.UTF8, "ContentType", "application/gzip;	name=\"embltd.co.uk!lincolnshire.gov.uk!1520846256!1520935058!827.xml.gz\""));

            string contentType = _contentTypeProvider.GetContentType(mimePart);

            Assert.That(contentType, Is.EqualTo(ContentType.ApplicationOctetStream));
        }

        [Test]
        public void ReturnedContentTypeIsLowerCase()
        {
            MimePart mimePart = CreateMimePart("TeXt", "PlAiN", "test.gz");

            string contentType = _contentTypeProvider.GetContentType(mimePart);

            Assert.That(contentType, Is.EqualTo(ContentType.TextPlain));
        }

        private MimePart CreateMimePart(string mediaType, string mediaSubtype, string filename, params Header[] headers)
        {
            var contentType = new MimeKit.ContentType(mediaType, mediaSubtype);

            IMimeContent contentObject = A.Fake<IMimeContent>();
            A.CallTo(() => contentObject.Stream).Returns(new MemoryStream());

            MimePart mimePart = new MimePart(contentType)
            {
                FileName = filename,
                Content = contentObject
            };

            foreach (Header header in headers)
            {
                mimePart.Headers.Add(header);
            }

            return mimePart;
        }
    }
}
