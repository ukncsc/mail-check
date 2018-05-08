using System;
using System.Collections.Generic;
using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.Rfc822.Headers
{
    [TestFixture]
    public class MailAddressParserTests
    {
        private IMailAddressConverter _mailAddressConverter;
        private MailAddressParser _mailAddressParser;

        [SetUp]
        public void SetUp()
        {
            _mailAddressConverter = A.Fake<IMailAddressConverter>();
            _mailAddressParser = new MailAddressParser(_mailAddressConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "user1@gov.uk" } } };

            MailAddress address = new MailAddress("user1@gov.uk");
            A.CallTo(() => _mailAddressConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(address);

            MailAddress mailAddress = _mailAddressParser.Parse(headers, "header1", false, false, false);

            Assert.That(mailAddress, Is.EqualTo(address));
        }

        [Test]
        public void FieldDoenstExistReturnsNull()
        {
            MailAddress mailAddress = _mailAddressParser.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(mailAddress, Is.Null);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _mailAddressParser.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void MultipleValuesThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "user1@gov.uk", "user2@gov.uk" } } };
            Assert.Throws<InvalidOperationException>(() => _mailAddressParser.Parse(headers, "header1", false, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<InvalidOperationException>(() => _mailAddressParser.Parse(headers, "header1", false, true, false));
        }
    }
}
