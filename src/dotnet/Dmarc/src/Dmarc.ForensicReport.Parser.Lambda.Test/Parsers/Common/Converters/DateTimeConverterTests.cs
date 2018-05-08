using System;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common.Converters
{
    [TestFixture]
    public class DateTimeConverterTests
    {
        private DateTimeConverter _dateTimeConverter;

        [SetUp]
        public void SetUp()
        {
            _dateTimeConverter = new DateTimeConverter(A.Fake<ILogger>());
        }

        [Test]
        public void ValidValueCorrectlyConverted()
        {
            DateTime? dateTime = _dateTimeConverter.Convert("Fri, 30 Mar 2017 08:00:00 -0800", "", false);
            Assert.That(dateTime, Is.EqualTo(new DateTime(2017, 3, 30, 16, 0, 0)));
        }

        [Test]
        public void InvalidValueReturnsNull()
        {
            DateTime? dateTime = _dateTimeConverter.Convert("asdfasdf", "", false);
            Assert.That(dateTime, Is.Null);
        }

        [Test]
        public void InvalidValueWithMandatoryConvertThrows()
        {
            Assert.Throws<ArgumentException>(() => _dateTimeConverter.Convert("asdfasdf", "", true));
        }
    }
}
