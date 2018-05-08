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
    public class MailAddressConverterTests
    {
        private MailAddressConverter _mailAddressConverter;

        [SetUp]
        public void SetUp()
        {
            _mailAddressConverter = new MailAddressConverter(A.Fake<ILogger>());
        }

        [Test]
        public void ValidValueCorrectlyConverted()
        {
            MailAddress mailAddress = _mailAddressConverter.Convert("user1@gov.uk", "", false);
            Assert.That(mailAddress.Address, Is.EqualTo("user1@gov.uk"));
        }

        [Test]
        public void InvalidValueReturnsNull()
        {
            MailAddress mailAddress = _mailAddressConverter.Convert("user1@gov.uk", "", false);
            Assert.That(mailAddress.Address, Is.EqualTo("user1@gov.uk"));
        }

        [Test]
        public void InvalidValueWithMandatoryConvertThrows()
        {
            Assert.Throws<ArgumentException>(() => _mailAddressConverter.Convert("asfdas", "", true));
        }
    }
}
