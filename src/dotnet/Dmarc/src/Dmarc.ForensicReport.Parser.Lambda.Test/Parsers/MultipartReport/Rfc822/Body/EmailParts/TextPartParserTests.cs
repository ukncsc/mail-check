using System.Collections.Generic;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Hashing;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body.EmailParts;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Urls;
using FakeItEasy;
using MimeKit;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.Rfc822.Body.EmailParts
{
    [TestFixture]
    public class TextPartParserTests
    {
        private TextPartParser _textPartParser;
        private IHashInfoCalculator _hashInfoCalculator;
        private IUrlExtractor _urlExtractor;

        [SetUp]
        public void SetUp()
        {
            _hashInfoCalculator = A.Fake<IHashInfoCalculator>();
            _urlExtractor = A.Fake<IUrlExtractor>();
            _textPartParser = new TextPartParser(new List<IHashInfoCalculator> {_hashInfoCalculator}, _urlExtractor);
        }

        [Test]
        public void TextPartCorrectlyConvertedToTextContent()
        {
            string mediaSubType = "html";
            string content = "<html><head></head><body></body></html>";
            bool isAttachment = false;
            int depth = 0;
            string hash = "Ad6Kj9Id";
            string url = "http://domain.gov.uk";

            TextPart textPart = Create(mediaSubType, content, isAttachment);

            A.CallTo(() => _hashInfoCalculator.Calculate(textPart)).Returns(new HashInfo {Hash = hash});
            A.CallTo(() => _urlExtractor.ExtractUrls(content)).Returns(new List<string> { url });

            TextContent textContent = _textPartParser.Parse(textPart, depth);

            Assert.That(textContent.Depth, Is.EqualTo(depth));
            Assert.That(textContent.ContentType, Is.EqualTo($"text/{mediaSubType}"));
            Assert.That(textContent.Disposition.IsAttachment, Is.EqualTo(isAttachment));
            Assert.That(textContent.RawContent, Is.EqualTo(content));
            Assert.That(textContent.Hashes.Count, Is.EqualTo(1));
            Assert.That(textContent.Hashes[0].Hash, Is.EqualTo(hash));
            Assert.That(textContent.Urls.Count, Is.EqualTo(1));
            Assert.That(textContent.Urls[0], Is.EqualTo(url));
        }

        private TextPart Create(string mediaSubType, string content, bool isAttachment)
        {
            return new TextPart(mediaSubType)
            {
                Text = content,
                ContentDisposition = new ContentDisposition
                {
                    IsAttachment = isAttachment
                }
            };
        }
    }
}
