using System;
using System.Collections.Generic;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common
{
    [TestFixture]
    public class RawValueParserTests
    {
        private RawValueParser _rawValueParser;

        [SetUp]
        public void SetUp()
        {
            _rawValueParser = new RawValueParser();
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            string expected = "Text";
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { expected } } };

            string rawValue = _rawValueParser.Parse(headers, "header1", false, false, false);

            Assert.That(rawValue, Is.EqualTo(expected));
        }

        [Test]
        public void FieldDoenstExistReturnsNull()
        {
            string rawValue = _rawValueParser.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(rawValue, Is.Null);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _rawValueParser.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void MultipleValuesThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "Text1", "Text2" } } };
            Assert.Throws<InvalidOperationException>(() => _rawValueParser.Parse(headers, "header1", false, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<InvalidOperationException>(() => _rawValueParser.Parse(headers, "header1", false, true, false));
        }
    }
}
