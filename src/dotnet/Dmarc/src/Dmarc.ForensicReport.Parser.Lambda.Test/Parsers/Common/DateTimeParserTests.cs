using System;
using System.Collections.Generic;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common
{
    [TestFixture]
    public class DateTimeParserTests
    {
        private DateTimeParser _dateTimeParser;
        private IDateTimeConverter _dateTimeConverter;

        [SetUp]
        public void SetUp()
        {
            _dateTimeConverter = A.Fake<IDateTimeConverter>();
            _dateTimeParser = new DateTimeParser(_dateTimeConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { {"header1", new List<string> { "30 Mar 2017 15:00:00 +0000" } } };

            DateTime expected = new DateTime(2017,3,30);
            A.CallTo(() => _dateTimeConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(expected);

            DateTime? dateTime = _dateTimeParser.Parse(headers, "header1", false, false, false);

            Assert.That(dateTime, Is.EqualTo(expected));
        }

        [Test]
        public void FieldDoenstExistReturnsNull()
        {
            DateTime? dateTime = _dateTimeParser.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(dateTime, Is.Null);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _dateTimeParser.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void MultipleValuesThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "30 Mar 2017 15:00:00 +0000", "30 Mar 2017 16:00:00 +0000" } } };
            Assert.Throws<InvalidOperationException>(() => _dateTimeParser.Parse(headers, "header1", false, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<InvalidOperationException>(() => _dateTimeParser.Parse(headers, "header1", false, true, false));
        }
    }
}
