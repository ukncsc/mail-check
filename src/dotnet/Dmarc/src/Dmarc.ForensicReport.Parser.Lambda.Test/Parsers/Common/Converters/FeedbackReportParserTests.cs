using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers;
using FakeItEasy;
using MimeKit;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common.Converters
{
    [TestFixture]
    public class FeedbackReportParserTests
    {
        private FeedbackReportParser _feedbackReportParser;
        private IFeedbackTypeParser _feedbackTypeParser;
        private IRawValueParser _rawValueParser;
        private IAuthFailureParser _authFailureParser;
        private IMailAddressCollectionParser _mailAddressCollectionParser;
        private IDateTimeParser _dateTimeParser;
        private IIntParser _intParser;
        private IDeliveryResultParser _deliveryResultParser;
        private IBase64Parser _base64Parser;
        private IRawValueParserMulti _rawValueParserMulti;
        private IMailAddressParser _mailAddressParser;
        private IMailAddressParserMulti _mailAddressParserMulti;
        private IIpAddressParser _ipAddressParser;

        [SetUp]
        public void SetUp()
        {
            _feedbackTypeParser = A.Fake<IFeedbackTypeParser>();
            _rawValueParser = A.Fake<IRawValueParser>();
            _authFailureParser = A.Fake<IAuthFailureParser>();
            _mailAddressCollectionParser = A.Fake<IMailAddressCollectionParser>();
            _dateTimeParser = A.Fake<IDateTimeParser>();
            _intParser = A.Fake<IIntParser>();
            _deliveryResultParser = A.Fake<IDeliveryResultParser>();
            _base64Parser = A.Fake<IBase64Parser>();
            _rawValueParserMulti = A.Fake<IRawValueParserMulti>();
            _mailAddressParser = A.Fake<IMailAddressParser>();
            _mailAddressParserMulti = A.Fake<IMailAddressParserMulti>();
            _ipAddressParser = A.Fake<IIpAddressParser>();
            _feedbackReportParser = new FeedbackReportParser(
                _feedbackTypeParser,
                _rawValueParser,
                _authFailureParser,
                _mailAddressCollectionParser,
                _dateTimeParser,
                _intParser,
                _deliveryResultParser,
                _base64Parser,
                _rawValueParserMulti,
                _mailAddressParser,
                _mailAddressParserMulti,
                _ipAddressParser);
        }

        [Test]
        public void CorrectValuesAssignedToFeedbackReport()
        {
            FeedbackType feedbackType = FeedbackType.AuthFailure;
            string userAgent = "DmarcReporter/1.0";
            string version = "1.0";
            string reportingMta = "dns;gov.uk";
            string originalEnvelopeId = "UaCwECpT8TjBzpTbRCeAQ";
            string authenticationResults = "dmarc:fail";
            string domain = "gov.uk";
            string dkimId = "@gov.uk";
            string dkimSelector = "16a6";
            string spfDns = "txt:gov,uk";
            DateTime arrivalDate = new DateTime(2017,3,30);
            IPAddress sourceIp = IPAddress.Parse("127.0.0.1");
            List<MailAddress> originalRcptTo = new List<MailAddress> {new MailAddress("user1@gov.uk")};
            List<string> reportedUris = new List<string> {"http://domain.gov.uk"};
            AuthFailure authFailure = AuthFailure.Dmarc;
            MailAddressCollection originalMailFrom = new MailAddressCollection {new MailAddress("user2@gov.uk")};
            DeliveryResult deliveryResult = DeliveryResult.Delivered;
            MailAddress messageId = new MailAddress("321321321654@gov.uk");
            string dkimCanonicalizedHeader = "9ovoa2ot9s7mi1";
            string dkimCanconicalizedBody = "lai9ni8vvovgr";

            A.CallTo(() => _feedbackTypeParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(feedbackType);
            A.CallTo(() => _rawValueParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._))
                .ReturnsNextFromSequence(userAgent, version, reportingMta, originalEnvelopeId, authenticationResults, domain, domain, dkimId, dkimSelector, spfDns);
            A.CallTo(() => _dateTimeParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(arrivalDate);
            A.CallTo(() => _ipAddressParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(sourceIp);
            A.CallTo(() => _intParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(10);
            A.CallTo(() => _mailAddressParserMulti.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(originalRcptTo);
            A.CallTo(() => _rawValueParserMulti.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(reportedUris);
            A.CallTo(() => _authFailureParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(authFailure);
            A.CallTo(() => _mailAddressCollectionParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(originalMailFrom);
            A.CallTo(() => _deliveryResultParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(deliveryResult);
            A.CallTo(() => _mailAddressParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(messageId);
            A.CallTo(() => _base64Parser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).ReturnsNextFromSequence(dkimCanonicalizedHeader, dkimCanconicalizedBody);

            MimeEntity mimeEntity = new MimePart(new ContentType("message", "feedback-report")) {ContentObject = A.Fake<IContentObject>()};
            FeedbackReport feedbackReport = _feedbackReportParser.Parse(mimeEntity, 0);

            Assert.That(feedbackReport.FeedbackType, Is.EqualTo(feedbackType));
            Assert.That(feedbackReport.UserAgent, Is.EqualTo(userAgent));
            Assert.That(feedbackReport.Version, Is.EqualTo(version));
            Assert.That(feedbackReport.ArrivalDate, Is.EqualTo(arrivalDate));
            Assert.That(feedbackReport.ReportingMta, Is.EqualTo(reportingMta));
            Assert.That(feedbackReport.SourceIp, Is.EqualTo(sourceIp));
            Assert.That(feedbackReport.Indicents, Is.EqualTo(10));
            Assert.That(feedbackReport.OrginalRcptTos[0], Is.EqualTo(originalRcptTo[0]));
            Assert.That(feedbackReport.ReportedUris[0], Is.EqualTo(reportedUris[0]));
            Assert.That(feedbackReport.AuthFailure, Is.EqualTo(authFailure));
            Assert.That(feedbackReport.OriginalEnvelopeId, Is.EqualTo(originalEnvelopeId));
            Assert.That(feedbackReport.OriginalMailFrom[0], Is.EqualTo(originalMailFrom[0]));
            Assert.That(feedbackReport.DeliveryResult, Is.EqualTo(deliveryResult));
            Assert.That(feedbackReport.AuthenticationResults, Is.EqualTo(authenticationResults));
            Assert.That(feedbackReport.ReportedDomain, Is.EqualTo(domain));
            Assert.That(feedbackReport.MessageId, Is.EqualTo(messageId));
            Assert.That(feedbackReport.DkimDomain, Is.EqualTo(domain));
            Assert.That(feedbackReport.DkimIdentity, Is.EqualTo(dkimId));
            Assert.That(feedbackReport.DkimSelector, Is.EqualTo(dkimSelector));
            Assert.That(feedbackReport.DkimCanonicalizedHeader, Is.EqualTo(dkimCanonicalizedHeader));
            Assert.That(feedbackReport.DkimCanonicalizedBody, Is.EqualTo(dkimCanconicalizedBody));
            Assert.That(feedbackReport.SpfDns, Is.EqualTo(spfDns));
        }

        [Test]
        public void MustBeFeedbackReport()
        {
            MimeEntity mimeEntity = new MimePart(new ContentType("message","not-feedback-report"));
            Assert.Throws<ArgumentException>(() => _feedbackReportParser.Parse(mimeEntity, 0));
        }
    }
}
