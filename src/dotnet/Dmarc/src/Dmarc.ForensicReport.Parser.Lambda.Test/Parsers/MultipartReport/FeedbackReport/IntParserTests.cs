using System;
using System.Collections.Generic;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.FeedbackReport
{
    [TestFixture]
    public class IntParserTests
    {
        private IIntConverter _intConverter;
        private IntParser _intParser;

        [SetUp]
        public void SetUp()
        {
            _intConverter = A.Fake<IIntConverter>();
            _intParser = new IntParser(_intConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "1" } } };

            int expected = 1;
            A.CallTo(() => _intConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(expected);

            int? value  = _intParser.Parse(headers, "header1", false, false, false);

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void FieldDoenstExistReturnsNull()
        {
            int? value  = _intParser.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(value, Is.Null);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _intParser.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void MultipleValuesThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "1", "1" } } };
            Assert.Throws<InvalidOperationException>(() => _intParser.Parse(headers, "header1", false, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<InvalidOperationException>(() => _intParser.Parse(headers, "header1", false, true, false));
        }
    }
}
