using System;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport;
using MimeKit;
using NUnit.Framework;
using Multipart = Dmarc.ForensicReport.Parser.Lambda.Domain.Multipart;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport
{
    [TestFixture]
    public class MultipartReportParserTests
    {
        private MultipartReportParser _multipartReportParser;

        [SetUp]
        public void SetUp()
        {
            _multipartReportParser = new MultipartReportParser();
        }

        [Test]
        public void ParseMultipartReportReturnsMultipart()
        {
            MimeKit.MultipartReport multipartReport = new MimeKit.MultipartReport("feedback-report");
            multipartReport.Add(new MimePart());
            multipartReport.Add(new MimePart());
            multipartReport.Add(new MimePart());

            Multipart multipart = _multipartReportParser.Parse(multipartReport , 0);
            Assert.That(multipart.ContentType, Is.EqualTo("multipart/report"));
            Assert.That(multipart.Disposition, Is.Null);
            Assert.That(multipart.Depth, Is.EqualTo(0));
        }

        [Test]
        public void NotMultipartThrows()
        {
            Assert.Throws<ArgumentException>(() => _multipartReportParser.Parse(new MessagePart("rfc822Message"), 0));
        }

        [Test]
        public void NotMultipartReportThrows()
        {
            MimeKit.MultipartReport multipartReport = new MimeKit.MultipartReport("not-feedback-report");
            multipartReport.Add(new MimePart());
            multipartReport.Add(new MimePart());
            multipartReport.Add(new MimePart());

            Assert.Throws<ArgumentException>(() => _multipartReportParser.Parse(multipartReport, 0));
        }

        [Test]
        public void MultipartReportDoesntContain3PartThrows()
        {
            MimeKit.MultipartReport multipartReport = new MimeKit.MultipartReport("feedback-report");
            multipartReport.Add(new MimePart());
            multipartReport.Add(new MimePart());

            Assert.Throws<ArgumentException>(() => _multipartReportParser.Parse(multipartReport, 0));
        }
    }
}
