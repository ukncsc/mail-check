using System;
using System.Net;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common.Converters
{
    [TestFixture]
    public class IpAddressConverterTests
    {
        private IPAddressConverter _ipAddressConverter;

        [SetUp]
        public void SetUp()
        {
            _ipAddressConverter = new IPAddressConverter(A.Fake<ILogger>());    
        }

        [Test]
        public void ValidIpV4AddressCorrectlyConverted()
        {
            IPAddress ipAddress = _ipAddressConverter.Convert("127.0.0.1", "", false);
            Assert.That(ipAddress.ToString(), Is.EqualTo("127.0.0.1"));
        }

        [Test]
        public void ValidIpV6AddressCorrectlyConverted()
        {
            IPAddress ipAddress = _ipAddressConverter.Convert("fe80::b96a:c41a:2f51:57b8", "", false);
            Assert.That(ipAddress.ToString(), Is.EqualTo("fe80::b96a:c41a:2f51:57b8"));
        }

        [Test]
        public void InvalidAddressReturnsNull()
        {
            IPAddress ipAddress = _ipAddressConverter.Convert("asdfads", "", false);
            Assert.That(ipAddress, Is.Null);
        }

        [Test]
        public void InvalidAddressWithMandatoryConvertThrows()
        {
            Assert.Throws<ArgumentException>(() => _ipAddressConverter.Convert("asdfads", "", true));
        }
    }
}
