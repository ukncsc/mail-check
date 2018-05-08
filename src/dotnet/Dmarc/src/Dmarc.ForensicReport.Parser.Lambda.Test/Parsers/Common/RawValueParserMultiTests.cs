using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common
{
    [TestFixture]
    public class RawValueParserMultiTests
    {
        private RawValueParserMulti _rawValueParserMulti;

        [SetUp]
        public void SetUp()
        {
            _rawValueParserMulti = new RawValueParserMulti();
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            List<string> expected = new List<string> { "Text1", "Text2" };
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", expected } };

            List<string> rawValues = _rawValueParserMulti.Parse(headers, "header1", false, false, false);

            Assert.That(rawValues.SequenceEqual(expected), Is.True);
        }

        [Test]
        public void MultipleValuesCorrectlyExtracted()
        {
            List<string> expected = new List<string> { "Text1","Text2" };
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", expected } };

            List<string> rawValues = _rawValueParserMulti.Parse(headers, "header1", false, false, false);

            Assert.That(rawValues.SequenceEqual(expected), Is.True);
        }

        [Test]
        public void FieldDoenstExistReturnsEmptyList()
        {
            List<string> expected = new List<string>();
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", expected } };

            List<string> rawValues = _rawValueParserMulti.Parse(headers, "header1", false, false, false);

            Assert.That(rawValues.SequenceEqual(expected), Is.True);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _rawValueParserMulti.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<ArgumentException>(() => _rawValueParserMulti.Parse(headers, "header1", false, true, false));
        }
    }
}
