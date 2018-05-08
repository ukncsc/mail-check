using System;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body.EmailParts;
using MimeKit;
using NUnit.Framework;
using Multipart = Dmarc.ForensicReport.Parser.Lambda.Domain.Multipart;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.Rfc822.Body.EmailParts
{
    [TestFixture]
    public class MultipartParserTests
    {
        private MultipartParser _multipartParser;

        [SetUp]
        public void Setup()
        {
            _multipartParser = new MultipartParser();
        }

        [Test]
        public void MultipartCorrectlyConvertedToMultipart()
        {
            bool isAttachment = false;
            int depth = 0;
            string mediaType = "multipart";
            string mediaSubtype = "report";
            MimeEntity mimeEntity = Create(mediaType, mediaSubtype, isAttachment);
            Multipart multipart = _multipartParser.Parse(mimeEntity, depth);

            Assert.That(multipart.Disposition.IsAttachment, Is.EqualTo(isAttachment));
            Assert.That(multipart.Depth, Is.EqualTo(depth));
            Assert.That(multipart.ContentType, Is.EqualTo($"{mediaType}/{mediaSubtype}"));
        }

        [Test]
        public void MustBeMultiPart()
        {
            MimeEntity mimeEntity = Create("text", "html", false);

            Assert.Throws<ArgumentException>(() => _multipartParser.Parse(mimeEntity, 0));
        }

        private MimeEntity Create(string mediaType, string mediaSubtype, bool isAttachment)
        {
            MimeEntity mimeEntity = new MimePart(new ContentType(mediaType, mediaSubtype))
            {
                ContentDisposition = new ContentDisposition
                {
                    IsAttachment = isAttachment
                }
            };

            return mimeEntity;
        }
    }
}
