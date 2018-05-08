using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.Rfc822.Headers
{
    [TestFixture]
    public class MailAddressCollectionParserMultiTests
    {
        private IMailAddressCollectionConverter _mailAddressCollectionConverter;
        private MailAddressCollectionParserMulti _mailAddressCollectionParserMulti;

        [SetUp]
        public void SetUp()
        {
            _mailAddressCollectionConverter = A.Fake<IMailAddressCollectionConverter>();
            _mailAddressCollectionParserMulti = new MailAddressCollectionParserMulti(_mailAddressCollectionConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "user1@gov.uk" } } };

            MailAddressCollection addressCollection = new MailAddressCollection { new MailAddress("user1@gov.uk") };
            A.CallTo(() => _mailAddressCollectionConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(addressCollection);

            MailAddressCollection mailAddressCollection = _mailAddressCollectionParserMulti.Parse(headers, "header1", false, false, false);

            Assert.That(mailAddressCollection.Count, Is.EqualTo(1));
            Assert.That(mailAddressCollection.SequenceEqual(addressCollection), Is.True);
        }

        [Test]
        public void MultipleValuesParsedCorrectly()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "user1@gov.uk, user2@gov.uk", "user3@gov.uk" } } };

            MailAddress mailAddress1 = new MailAddress("user1@gov.uk");
            MailAddress mailAddress2 = new MailAddress("user2@gov.uk");
            MailAddress mailAddress3 = new MailAddress("user3@gov.uk");

            MailAddressCollection addressCollection1 = new MailAddressCollection { mailAddress1, mailAddress2 };
            MailAddressCollection addressCollection2 = new MailAddressCollection { mailAddress3 };

            A.CallTo(() => _mailAddressCollectionConverter.Convert(A<string>._, A<string>._, A<bool>._)).ReturnsNextFromSequence(addressCollection1, addressCollection2);

            MailAddressCollection mailAddressCollection = _mailAddressCollectionParserMulti.Parse(headers, "header1", false, false, false);

            Assert.That(mailAddressCollection.Count, Is.EqualTo(3));
            Assert.That(mailAddressCollection[0], Is.EqualTo(mailAddress1));
            Assert.That(mailAddressCollection[1], Is.EqualTo(mailAddress2));
            Assert.That(mailAddressCollection[2], Is.EqualTo(mailAddress3));
        }

        [Test]
        public void FieldDoenstExistReturnsNull()
        {
            MailAddressCollection mailAddressCollection = _mailAddressCollectionParserMulti.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(mailAddressCollection, Is.Null);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _mailAddressCollectionParserMulti.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<ArgumentException>(() => _mailAddressCollectionParserMulti.Parse(headers, "header1", false, true, false));
        }
    }
}
