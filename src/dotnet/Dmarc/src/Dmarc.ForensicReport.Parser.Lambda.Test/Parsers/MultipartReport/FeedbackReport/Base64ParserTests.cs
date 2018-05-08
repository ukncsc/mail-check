using System;
using System.Collections.Generic;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.FeedbackReport
{
    [TestFixture]
    public class Base64ParserTests
    {
        private Base64Parser _base64Parser;

        [SetUp]
        public void SetUp()
        {
            _base64Parser = new Base64Parser();
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            string headerBase64String = "vNDNkSVZzVsdCYgA1aFd==";
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { headerBase64String } } };

            string base64String = _base64Parser.Parse(headers, "header1", false, false, false);

            Assert.That(base64String, Is.EqualTo(headerBase64String));
        }

        [Test]
        public void FieldAndValueExistWithWhiteSpaceReturnsValueWithoutWhiteSpace()
        {
            string headerBase64String = "vNDNkSVZz \r\nVsdCYgA1aFd==";
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { headerBase64String } } };

            string base64String = _base64Parser.Parse(headers, "header1", false, false, false);

            Assert.That(base64String, Is.EqualTo("vNDNkSVZzVsdCYgA1aFd=="));
        }

        [Test]
        public void FieldDoesntExistReturnsNull()
        {
            string base64String = _base64Parser.Parse(new Dictionary<string, List<string>>(), "header1", false, false, false);

            Assert.That(base64String, Is.Null);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _base64Parser.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void MultipleValuesThrows()
        {
            string headerBase64String = "vNDNkSVZzVsdCYgA1aFd==";
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { headerBase64String, headerBase64String } } };

            Assert.Throws<InvalidOperationException>(() => _base64Parser.Parse(headers, "header1", true, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };

            Assert.Throws<InvalidOperationException>(() => _base64Parser.Parse(headers, "header1", false, true, false));
        }
    }
}
