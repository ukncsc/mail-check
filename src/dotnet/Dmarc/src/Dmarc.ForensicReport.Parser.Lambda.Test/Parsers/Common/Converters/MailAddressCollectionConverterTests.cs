using System;
using System.Net.Mail;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common.Converters
{
    [TestFixture]
    public class MailAddressCollectionConverterTests
    {
        private MailAddressCollectionConverter _mailAddressCollectionConverter;

        [SetUp]
        public void SetUp()
        {
            _mailAddressCollectionConverter = new MailAddressCollectionConverter(new MailAddressConverter(A.Fake<ILogger>()));
        }

        [Test]
        public void ValidValueCorrectlyConverted()
        {
            MailAddressCollection mailAddressCollection = _mailAddressCollectionConverter.Convert("User1 <user1@gov.uk>, user2@gov.uk", "", false);
            Assert.That(mailAddressCollection.Count, Is.EqualTo(2));
            Assert.That(mailAddressCollection[0].Address, Is.EqualTo("user1@gov.uk"));
            Assert.That(mailAddressCollection[1].Address, Is.EqualTo("user2@gov.uk"));
        }

        [Test]
        public void ValidValueOfGroupCorrectlyConverted()
        {
            MailAddressCollection mailAddressCollection = _mailAddressCollectionConverter.Convert("A Group:User1 <user1@gov.uk>,user2@gov.uk,User3 <user3@gov.uk>;", "", false);
            Assert.That(mailAddressCollection.Count, Is.EqualTo(3));
            Assert.That(mailAddressCollection[0].Address, Is.EqualTo("user1@gov.uk"));
            Assert.That(mailAddressCollection[1].Address, Is.EqualTo("user2@gov.uk"));
            Assert.That(mailAddressCollection[2].Address, Is.EqualTo("user3@gov.uk"));
        }

        [Test]
        public void ValidValueOfSingleAndGroupCorrectlyConverted()
        {
            MailAddressCollection mailAddressCollection = _mailAddressCollectionConverter.Convert("A Group:User1 <user1@gov.uk>,user2@gov.uk,User3 <user3@gov.uk>;user4@gov.uk", "", false);
            Assert.That(mailAddressCollection.Count, Is.EqualTo(4));
            Assert.That(mailAddressCollection[0].Address, Is.EqualTo("user1@gov.uk"));
            Assert.That(mailAddressCollection[1].Address, Is.EqualTo("user2@gov.uk"));
            Assert.That(mailAddressCollection[2].Address, Is.EqualTo("user3@gov.uk"));
            Assert.That(mailAddressCollection[3].Address, Is.EqualTo("user4@gov.uk"));
        }

        [Test]
        public void InvalidValueReturnsEmptyList()
        {
            MailAddressCollection mailAddressCollection = _mailAddressCollectionConverter.Convert("asfdas", "", false);
            Assert.That(mailAddressCollection.Count, Is.EqualTo(0));            
        }

        [Test]
        public void InvalidValuesIgnored()
        {
            MailAddressCollection mailAddressCollection = _mailAddressCollectionConverter.Convert("asfdas, user1@gov.uk", "", false);
            Assert.That(mailAddressCollection.Count, Is.EqualTo(1));
            Assert.That(mailAddressCollection[0].Address, Is.EqualTo("user1@gov.uk"));
        }

        [Test]
        public void InvalidValueWithMandatoryConvertThrows()
        {
            Assert.Throws<ArgumentException>(() => _mailAddressCollectionConverter.Convert("asfdas", "", true));
        }
    }
}
