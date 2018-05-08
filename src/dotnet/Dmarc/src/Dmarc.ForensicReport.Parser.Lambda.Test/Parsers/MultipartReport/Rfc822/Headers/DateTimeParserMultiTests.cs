using System;
using System.Collections.Generic;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.Rfc822.Headers
{
    [TestFixture]
    public class DateTimeParserMultiTests
    {
        private IDateTimeConverter _dateTimeConverter;
        private DateTimeParserMulti _dateTimeParserMulti;

        [SetUp]
        public void SetUp()
        {
            _dateTimeConverter = A.Fake<IDateTimeConverter>();
            _dateTimeParserMulti = new DateTimeParserMulti(_dateTimeConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "30 Mar 2017 15:00:00 +0000" } } };

            DateTime expected = new DateTime(2017, 3, 30);
            A.CallTo(() => _dateTimeConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(expected);

            List<DateTime> dateTimes = _dateTimeParserMulti.Parse(headers, "header1", false, false, false);

            Assert.That(dateTimes.Count, Is.EqualTo(1));
            Assert.That(dateTimes[0], Is.EqualTo(expected));
            A.CallTo(() => _dateTimeConverter.Convert(A<string>._, A<string>._, A<bool>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void MultipleValuesCorrectlyReturned()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "30 Mar 2017 15:00:00 +0000", "30 Mar 2017 15:00:00 +0000" } } };

            DateTime expected1 = new DateTime(2017, 3, 30);
            DateTime expected2 = new DateTime(2017, 3, 31);
            A.CallTo(() => _dateTimeConverter.Convert(A<string>._, A<string>._, A<bool>._)).ReturnsNextFromSequence(expected1, expected2);

            List<DateTime> dateTimes = _dateTimeParserMulti.Parse(headers, "header1", false, false, false);

            Assert.That(dateTimes.Count, Is.EqualTo(2));
            Assert.That(dateTimes[0], Is.EqualTo(expected1));
            Assert.That(dateTimes[1], Is.EqualTo(expected2));

            A.CallTo(() => _dateTimeConverter.Convert(A<string>._, A<string>._, A<bool>._)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void FieldDoenstExistReturnsEmptyList()
        {
            List<DateTime> dateTimes = _dateTimeParserMulti.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(dateTimes, Is.Empty);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _dateTimeParserMulti.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<ArgumentException>(() => _dateTimeParserMulti.Parse(headers, "header1", false, true, false));
        }

    }
}
