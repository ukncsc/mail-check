using System;
using System.Collections.Generic;
using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common
{
    [TestFixture]
    public class MailAddressParserMultiTests
    {
        private IMailAddressConverter _mailAddressConverter;
        private MailAddressParserMulti _mailAddressParserMulti;

        [SetUp]
        public void SetUp()
        {
            _mailAddressConverter = A.Fake<IMailAddressConverter>();
            _mailAddressParserMulti = new MailAddressParserMulti(_mailAddressConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "user1@gov.uk" } } };


            MailAddress mailAddress = new MailAddress("user1@gov.uk");
            A.CallTo(() => _mailAddressConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(mailAddress);

            List<MailAddress> mailAddresses = _mailAddressParserMulti.Parse(headers, "header1", false, false, false);

            Assert.That(mailAddresses.Count, Is.EqualTo(1));
            Assert.That(mailAddresses[0], Is.EqualTo(mailAddress));
            A.CallTo(() => _mailAddressConverter.Convert(A<string>._, A<string>._, A<bool>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void MultipleValuesAllowed()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "user1@gov.uk", "user2@gov.uk" } } };

            MailAddress mailAddress1 = new MailAddress("user1@gov.uk");
            MailAddress mailAddress2 = new MailAddress("user2@gov.uk");
            A.CallTo(() => _mailAddressConverter.Convert(A<string>._, A<string>._, A<bool>._)).ReturnsNextFromSequence(mailAddress1, mailAddress2);

            List<MailAddress> mailAddresses = _mailAddressParserMulti.Parse(headers, "header1", false, false, false);

            Assert.That(mailAddresses.Count, Is.EqualTo(2));
            Assert.That(mailAddresses[0], Is.EqualTo(mailAddress1));
            Assert.That(mailAddresses[1], Is.EqualTo(mailAddress2));
            A.CallTo(() => _mailAddressConverter.Convert(A<string>._, A<string>._, A<bool>._)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void FieldDoenstExistReturnsEmptyList()
        {
            List<MailAddress> mailAddresses = _mailAddressParserMulti.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(mailAddresses, Is.Empty);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _mailAddressParserMulti.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<ArgumentException>(() => _mailAddressParserMulti.Parse(headers, "header1", false, true, false));
        }
    }
}
