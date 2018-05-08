using System;
using System.Collections.Generic;
using System.Net;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.Rfc822.Headers
{
    [TestFixture]
    public class XOriginatingIpAddressParserTests
    {
        private XOriginatingIPAddressParser _xOriginatingIpAddressParser;
        private IIPAddressConverter _ipAddressConverter;

        [SetUp]
        public void SetUp()
        {
            _ipAddressConverter = A.Fake<IIPAddressConverter>();
            _xOriginatingIpAddressParser = new XOriginatingIPAddressParser(_ipAddressConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "[127.0.0.1]" } } };

            IPAddress expected = IPAddress.Parse("127.0.0.1");
            A.CallTo(() => _ipAddressConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(expected);

            List<IPAddress> ipAddresses = _xOriginatingIpAddressParser.Parse(headers, "header1", false, false, false);

            Assert.That(ipAddresses.Count, Is.EqualTo(1));
            Assert.That(ipAddresses[0], Is.EqualTo(expected));
            A.CallTo(() => _ipAddressConverter.Convert(A<string>.That.Matches(_ => _ == "127.0.0.1"), A<string>._, A<bool>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void MultipleValuesParsedCorrectly()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "[127.0.0.1]", "[128.0.0.1]" } } };

            IPAddress expected1 = IPAddress.Parse("127.0.0.1");
            IPAddress expected2 = IPAddress.Parse("128.0.0.1");
            A.CallTo(() => _ipAddressConverter.Convert(A<string>._, A<string>._, A<bool>._)).ReturnsNextFromSequence(expected1,expected2);

            List<IPAddress> ipAddresses = _xOriginatingIpAddressParser.Parse(headers, "header1", false, false, false);

            Assert.That(ipAddresses.Count, Is.EqualTo(2));
            Assert.That(ipAddresses[0], Is.EqualTo(expected1));
            Assert.That(ipAddresses[1], Is.EqualTo(expected2));
            A.CallTo(() => _ipAddressConverter.Convert(A<string>.That.Matches(_ => _ == "127.0.0.1"), A<string>._, A<bool>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _ipAddressConverter.Convert(A<string>.That.Matches(_ => _ == "128.0.0.1"), A<string>._, A<bool>._)).MustHaveHappened(Repeated.Exactly.Once);

        }

        [Test]
        public void FieldDoenstExistReturnsNull()
        {
            List<IPAddress> ipAddresses = _xOriginatingIpAddressParser.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(ipAddresses, Is.Empty);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _xOriginatingIpAddressParser.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<ArgumentException>(() => _xOriginatingIpAddressParser.Parse(headers, "header1", false, true, false));
        }
    }
}
