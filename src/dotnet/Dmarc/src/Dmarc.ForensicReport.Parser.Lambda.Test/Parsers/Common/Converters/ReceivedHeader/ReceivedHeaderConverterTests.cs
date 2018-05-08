using System.Collections.Generic;
using System.Net.Mail;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters.ReceivedHeader;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common.Converters.ReceivedHeader
{
    [TestFixture]
    public class ReceivedHeaderConverterTests
    {
        private ReceivedHeaderConverter _receivedHeaderConverter;
        private IReceivedHeaderSplitter _receivedHeaderSplitter;
        private ILogger _logger;
        private IMailAddressConverter _mailAddressConverter;

        [SetUp]
        public void SetUp()
        {
            _receivedHeaderSplitter = A.Fake<IReceivedHeaderSplitter>();
            _logger = A.Fake<ILogger>();
            _mailAddressConverter = A.Fake<IMailAddressConverter>();
            _receivedHeaderConverter = new ReceivedHeaderConverter(_receivedHeaderSplitter,
                _mailAddressConverter,
                _logger);
        }

        [Test]
        public void ValidInputCorrectlyExtactsFromAndByFields()
        {
            string fromField = "a.b.c [127.0.0.1]";
            string byField = "d.e.f (128.0.0.1)";

            MailAddress forMailAddress = new MailAddress("a.b@gov.uk");

            A.CallTo(() => _receivedHeaderSplitter.Split(A<string>._)).Returns(new List<string> { "from", fromField, "by", byField, "for","<a.b@gov.uk>" });
            A.CallTo(() => _mailAddressConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(forMailAddress);

            Domain.ReceivedHeader receivedHeader = _receivedHeaderConverter.Convert("", "", true);

            Assert.That(receivedHeader.From, Is.EqualTo(fromField));
            Assert.That(receivedHeader.By, Is.EqualTo(byField));
            Assert.That(receivedHeader.For, Is.EqualTo(forMailAddress));
        }

        [Test]
        public void InvalidInputCorrectlyExtactsFromAndByFields()
        {
            string fromField = "a.b.c [127.0.0.1]";
            string byField = "d.e.f (128.0.0.1)";

            MailAddress forMailAddress = new MailAddress("a.b@gov.uk");

            A.CallTo(() => _receivedHeaderSplitter.Split(A<string>._)).Returns(new List<string> { fromField, byField, "<a.b@gov.uk>" });
            A.CallTo(() => _mailAddressConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(forMailAddress);

            Domain.ReceivedHeader receivedHeader = _receivedHeaderConverter.Convert("", "", true);

            Assert.That(receivedHeader.From, Is.Null);
            Assert.That(receivedHeader.By, Is.Null);
            Assert.That(receivedHeader.For, Is.Null);
        }
    }
}
