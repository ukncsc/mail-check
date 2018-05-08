using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers;
using FakeItEasy;
using MimeKit;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.Rfc822.Headers
{
    [TestFixture]
    public class Rfc822HeaderParserTests
    {
        private Rfc822HeadersParser _rfc822HeaderParser;
        private IMailAddressCollectionParser _mailAddressCollectionParser;
        private IMailAddressCollectionParserMulti _mailAddressCollectionParserMulti;
        private IMailAddressParserMulti _mailAddressParserMulti;
        private IMailAddressParser _mailAddressParser;
        private IRawValueParserMulti _rawValueParserMulti;
        private IRawValueParser _rawValueParser;
        private IDateTimeParserMulti _dateTimeParserMulti;
        private IDateTimeParser _dateTimeParser;
        private IXOriginatingIPAddressParser _xOriginatingIpAddressParser;
        private IReceivedHeaderParserMulti _receivedHeaderParserMulti;

        [SetUp]
        public void SetUp()
        {
            _mailAddressCollectionParser = A.Fake<IMailAddressCollectionParser>();
            _mailAddressCollectionParserMulti = A.Fake<IMailAddressCollectionParserMulti>();
            _mailAddressParserMulti = A.Fake<IMailAddressParserMulti>();
            _mailAddressParser = A.Fake<IMailAddressParser>();
            _rawValueParserMulti = A.Fake<IRawValueParserMulti>();
            _rawValueParser = A.Fake<IRawValueParser>();
            _dateTimeParserMulti = A.Fake<IDateTimeParserMulti>();
            _dateTimeParser = A.Fake<IDateTimeParser>();
            _xOriginatingIpAddressParser = A.Fake<IXOriginatingIPAddressParser>();
            _receivedHeaderParserMulti = A.Fake<IReceivedHeaderParserMulti>();
            _rfc822HeaderParser = new Rfc822HeadersParser(
                _mailAddressCollectionParser,
                _mailAddressCollectionParserMulti,
                _mailAddressParserMulti,
                _mailAddressParser,
                _rawValueParserMulti,
                _rawValueParser,
                _dateTimeParserMulti,
                _dateTimeParser,
                _xOriginatingIpAddressParser,
                _receivedHeaderParserMulti
                );
        }

        [Test]
        public void ContentTypeMustBeRfc822()
        {
            MimeEntity mimeEntity = new MessagePart("not-rfc822");
            Assert.Throws<ArgumentException>(() => _rfc822HeaderParser.Parse(mimeEntity, 0));
        }

        [Test]
        public void ContentTypeMustBeRfc822Headers()
        {
            MimeEntity mimeEntity = new TextPart("not-rfc822-headers");
            Assert.Throws<ArgumentException>(() => _rfc822HeaderParser.Parse(mimeEntity, 0));
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public void CorrectValuesAreAssignedToRfc822Fields(MimeEntity mimeEntity)
        {
            DateTime date = new DateTime(2017, 4, 3);
            List<DateTime> resentDate = new List<DateTime> { new DateTime(2017, 4, 3)};
            List<MailAddress> returnPath = new List<MailAddress> {new MailAddress("user1@gov.uk")};
            MailAddressCollection from = new MailAddressCollection { new MailAddress("user3@gov.uk") };
            MailAddressCollection resentFrom = new MailAddressCollection { new MailAddress("user10@gov.uk") };
            MailAddress sender = new MailAddress("user15@gov.uk");
            MailAddressCollection resentSender = new MailAddressCollection { new MailAddress("user11@gov.uk") };
            MailAddressCollection to = new MailAddressCollection { new MailAddress("user4@gov.uk") };
            MailAddressCollection resentTo = new MailAddressCollection { new MailAddress("user12@gov.uk") };
            MailAddressCollection replyTo = new MailAddressCollection { new MailAddress("user5@gov.uk") };
            MailAddressCollection cc = new MailAddressCollection { new MailAddress("user6@gov.uk") };
            MailAddressCollection resentCc = new MailAddressCollection { new MailAddress("user13@gov.uk") };
            MailAddressCollection bcc = new MailAddressCollection { new MailAddress("user7@gov.uk") };
            MailAddressCollection resentBcc = new MailAddressCollection { new MailAddress("user14@gov.uk") };
            MailAddress messageId = new MailAddress("user16@gov.uk");
            List<MailAddress> resentMessageId = new List<MailAddress> { new MailAddress("user2@gov.uk") };
            MailAddressCollection inReplyTo = new MailAddressCollection { new MailAddress("user8@gov.uk") };
            MailAddressCollection references = new MailAddressCollection { new MailAddress("user9@gov.uk") };
            string subject = "Subject";
            List<string> comments = new List<string> { "Comments" };
            List<string> keywords = new List<string> { "Keywords" };
            string authenticationResults = "dmarc: fail";
            string receivedSpf = "pass (spfCheck....";
            string userAgent = "Reporter/1.0";
            string dkimSignature = "v=1; a=rsa-sha256; c=relaxed/relaxed; d=gov.uk;";
            List<IPAddress> xOriginatingIpAddresses = new List<IPAddress> { IPAddress.Parse("127.0.0.1") };
            List<ReceivedHeader> receivedHeaders = new List<ReceivedHeader> {new ReceivedHeader("from a.b.c", "by d.e.f", new MailAddress("a.b@gov.uk")) };

            A.CallTo(() => _dateTimeParser.Parse(A<Dictionary<string,List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(date);
            A.CallTo(() => _dateTimeParserMulti.Parse(A<Dictionary<string,List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._)).Returns(resentDate);
            A.CallTo(() => _rawValueParserMulti.Parse(A<Dictionary<string,List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._))
                .ReturnsNextFromSequence (comments, keywords);

            A.CallTo(() => _rawValueParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._))
                .ReturnsNextFromSequence(subject, authenticationResults, receivedSpf, userAgent, dkimSignature);

            A.CallTo(() => _mailAddressParserMulti.Parse(A<Dictionary<string,List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._))
                .ReturnsNextFromSequence( returnPath, resentMessageId);
            
            A.CallTo(() => _mailAddressCollectionParser.Parse(A<Dictionary<string,List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._))
                .ReturnsNextFromSequence(from, to, replyTo, cc, bcc, inReplyTo, references);
            
            A.CallTo(() => _mailAddressCollectionParserMulti.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._))
                .ReturnsNextFromSequence( resentFrom, resentSender, resentTo, resentCc, resentBcc);
            
            A.CallTo(() => _mailAddressParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._))
                .ReturnsNextFromSequence( sender, messageId);
            
            A.CallTo(() => _xOriginatingIpAddressParser.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._))
                .Returns(xOriginatingIpAddresses);

            A.CallTo(() => _receivedHeaderParserMulti.Parse(A<Dictionary<string, List<string>>>._, A<string>._, A<bool>._, A<bool>._, A<bool>._))
                .Returns(receivedHeaders);

            Domain.Rfc822 rfc822 = _rfc822HeaderParser.Parse(mimeEntity, 0);

            Assert.That(rfc822.Date, Is.EqualTo(date));
            Assert.That(rfc822.ResentDate.SequenceEqual(resentDate), Is.True);
            Assert.That(rfc822.Received.SequenceEqual(receivedHeaders), Is.True);
            Assert.That(rfc822.ReturnPath.SequenceEqual(returnPath), Is.True);
            Assert.That(rfc822.From.SequenceEqual(from), Is.True);
            Assert.That(rfc822.ResentFrom.SequenceEqual(resentFrom), Is.True);
            Assert.That(rfc822.To.SequenceEqual(to), Is.True);
            Assert.That(rfc822.ResentTo.SequenceEqual(resentTo), Is.True);
            Assert.That(rfc822.ReplyTo.SequenceEqual(replyTo), Is.True);
            Assert.That(rfc822.Cc.SequenceEqual(cc), Is.True);
            Assert.That(rfc822.ResentCc.SequenceEqual(resentCc), Is.True);
            Assert.That(rfc822.Bcc.SequenceEqual(bcc), Is.True);
            Assert.That(rfc822.ResentBcc.SequenceEqual(resentBcc), Is.True);
            Assert.That(rfc822.MessageId, Is.EqualTo(rfc822.MessageId));
            Assert.That(rfc822.ResentMessageId.SequenceEqual(resentMessageId), Is.True);
            Assert.That(rfc822.InReplyTo.SequenceEqual(inReplyTo), Is.True);
            Assert.That(rfc822.References.SequenceEqual(references), Is.True);
            Assert.That(rfc822.Subject, Is.EqualTo(subject));
            Assert.That(rfc822.Comments.SequenceEqual(comments));
            Assert.That(rfc822.Keywords.SequenceEqual(keywords));
            Assert.That(rfc822.AuthenticationResults, Is.EqualTo(authenticationResults));
            Assert.That(rfc822.ReceivedSpf, Is.EqualTo(receivedSpf));
            Assert.That(rfc822.UserAgent, Is.EqualTo(userAgent));
            Assert.That(rfc822.DkimSignature, Is.EqualTo(dkimSignature));
            Assert.That(rfc822.XOrigninatingIp.SequenceEqual(xOriginatingIpAddresses));
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            yield return new TestCaseData(new MessagePart("rfc822"){Message = new MimeMessage()});

            IContentObject contentObject = A.Fake<IContentObject>();
            A.CallTo(() => contentObject.Open()).Returns(Stream.Null);
            yield return new TestCaseData(new TextPart("rfc822-headers") {ContentObject = contentObject});
        }
    }
}
