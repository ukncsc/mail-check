using System;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.HumanReadable;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body.EmailParts;
using FakeItEasy;
using MimeKit;
using NUnit.Framework;
using Multipart = MimeKit.Multipart;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.HumanReadable
{
    [TestFixture]
    public class ForensicReportTextPartParserTests
    {
        private ITextPartParser _textPartParser;
        private ForensicReportTextPartParser _forensicReportTextPastParser;

        [SetUp]
        public void SetUp()
        {
            _textPartParser = A.Fake<ITextPartParser>();
            _forensicReportTextPastParser = new ForensicReportTextPartParser(_textPartParser);
        }

        [Test]
        public void MimePartMustBeTextPart()
        {
            Assert.Throws<ArgumentException>(() => _forensicReportTextPastParser.Parse(new Multipart(), 0));
        }

        [Test]
        public void ContentTypeMustBePlainText()
        {
            Assert.Throws<ArgumentException>(() => _forensicReportTextPastParser.Parse(new TextPart("html"),0));
        }

        [Test]
        public void ParsingCorrectlyOccurs()
        {
            int depth = 0;
            TextPart textPart = new TextPart("plain");
            TextContent textContent = _forensicReportTextPastParser.Parse(textPart, depth);

            Assert.That(textContent, Is.Not.Null);
            A.CallTo(() => _textPartParser.Parse(textPart, depth)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
