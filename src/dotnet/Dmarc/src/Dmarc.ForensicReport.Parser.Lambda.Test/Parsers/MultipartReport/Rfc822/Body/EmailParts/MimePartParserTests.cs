using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Hashing;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body.EmailParts;
using FakeItEasy;
using MimeKit;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.Rfc822.Body.EmailParts
{
    [TestFixture]
    public class MimePartParserTests
    {
        private MimePartParser _mimePartParser;
        private IHashInfoCalculator _hashInfoCalculator;

        [SetUp]
        public void SetUp()
        {
            _hashInfoCalculator = A.Fake<IHashInfoCalculator>();
            _mimePartParser = new MimePartParser(new List<IHashInfoCalculator> {_hashInfoCalculator});
        }

        [Test]
        public void MimePartCorrectlyConvertedToMimeContent()
        {
            string streamContents = "A3Y8dKj6Hs";
            string filename = "file.pdf";
            bool isAttachement = true;
            string mediaType = "application";
            string mediaSubType = "octet-stream";
            string hash = "AdH67Df3";
            int depth = 0;

            MimePart mimePart = CreateMimePart(streamContents, mediaType, mediaSubType, filename, isAttachement);
            
            A.CallTo(() => _hashInfoCalculator.Calculate(mimePart)).Returns(new HashInfo{Hash = hash});

            MimeContent mimeContent = _mimePartParser.Parse(mimePart, depth);
            Assert.That(mimeContent.Depth, Is.EqualTo(depth));
            Assert.That(mimeContent.ContentType, Is.EqualTo($"{mediaType}/{mediaSubType}"));
            Assert.That(mimeContent.Disposition.IsAttachment, Is.EqualTo(isAttachement));
            Assert.That(mimeContent.Disposition.Filename, Is.EqualTo(filename));
            Assert.That(mimeContent.Hashes.Count, Is.EqualTo(1));
            Assert.That(mimeContent.Hashes[0].Hash, Is.EqualTo(hash));
            Assert.That(mimeContent.RawContent.SequenceEqual(Encoding.UTF8.GetBytes(streamContents)));
        }

        private MimePart CreateMimePart(string streamContents, string mediaType, string mediaSubType, string filename, bool isAttachement)
        {
            IContentObject contentObject = A.Fake<IContentObject>();
            A.CallTo(() => contentObject.Open()).Returns(new MemoryStream(Encoding.UTF8.GetBytes(streamContents)));
            return new MimePart(new ContentType(mediaType, mediaSubType))
            {
                ContentObject = contentObject,
                ContentDisposition = new ContentDisposition
                {
                    IsAttachment = isAttachement,
                    FileName = filename
                }
            };
        }
    }
}
