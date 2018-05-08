using System;
using System.Collections.Generic;
using System.Net;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.FeedbackReport
{
    [TestFixture]
    public class IpAddressParserTests
    {
        private IIPAddressConverter _ipAddressConverter;
        private IpAddressParser _ipAddressParser;

        [SetUp]
        public void SetUp()
        {
            _ipAddressConverter = A.Fake<IIPAddressConverter>();
            _ipAddressParser = new IpAddressParser(_ipAddressConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "127.0.0.1" } } };

            IPAddress expected = IPAddress.Parse("127.0.0.1");
            A.CallTo(() => _ipAddressConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(expected);

            IPAddress ipAddress = _ipAddressParser.Parse(headers, "header1", false, false, false);

            Assert.That(ipAddress, Is.EqualTo(expected));
        }

        [Test]
        public void FieldDoenstExistReturnsNull()
        {
            IPAddress ipAddress = _ipAddressParser.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(ipAddress, Is.Null);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _ipAddressParser.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void MultipleValuesThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "127.0.0.1", "127.0.0.1" } } };
            Assert.Throws<InvalidOperationException>(() => _ipAddressParser.Parse(headers, "header1", false, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<InvalidOperationException>(() => _ipAddressParser.Parse(headers, "header1", false, true, false));
        }
    }
}
