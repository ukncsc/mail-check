using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common
{
    [TestFixture]
    public class MailAddressCollectionParserTests
    {
        private IMailAddressCollectionConverter _mailAddressCollectionConverter;
        private MailAddressCollectionParser _mailAddressCollectionParser;

        [SetUp]
        public void SetUp()
        {
            _mailAddressCollectionConverter = A.Fake<IMailAddressCollectionConverter>();
            _mailAddressCollectionParser = new MailAddressCollectionParser(_mailAddressCollectionConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "user1@gov.uk" } } };

            MailAddressCollection addressCollection = new MailAddressCollection {new MailAddress("user1@gov.uk")};
            A.CallTo(() => _mailAddressCollectionConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(addressCollection);

            MailAddressCollection mailAddressCollection = _mailAddressCollectionParser.Parse(headers, "header1", false, false, false);

            Assert.That(mailAddressCollection.Count, Is.EqualTo(1));
            Assert.That(mailAddressCollection.SequenceEqual(addressCollection), Is.True);
        }

        [Test]
        public void FieldDoenstExistReturnsNull()
        {
            MailAddressCollection mailAddressCollection = _mailAddressCollectionParser.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(mailAddressCollection, Is.Null);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _mailAddressCollectionParser.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void MultipleValuesThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "user1@gov.uk", "user2@gov.uk" } } };
            Assert.Throws<InvalidOperationException>(() => _mailAddressCollectionParser.Parse(headers, "header1", false, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<InvalidOperationException>(() => _mailAddressCollectionParser.Parse(headers, "header1", false, true, false));
        }
    }
}
