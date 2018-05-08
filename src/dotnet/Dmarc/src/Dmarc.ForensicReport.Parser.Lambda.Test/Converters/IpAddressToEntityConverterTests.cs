using System.Net;
using Dmarc.ForensicReport.Parser.Lambda.Converters;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Converters
{
    [TestFixture]
    public class IpAddressToEntityConverterTests
    {
        private IpAddressToEntityConverter _ipAddressToEntityConverter;

        [SetUp]
        public void SetUp()
        {
            _ipAddressToEntityConverter = new IpAddressToEntityConverter();
        }

        [Test]
        public void ConvertCorrectlyConverts()
        {
            string ipString = "127.0.0.1";
            IPAddress ipAddress = IPAddress.Parse(ipString);
            IpAddressEntity ipAddressEntity = _ipAddressToEntityConverter.Convert(ipAddress);

            Assert.That(ipAddressEntity.Ip, Is.EqualTo(ipString));
            Assert.That(ipAddressEntity.BinaryIp, Is.EqualTo("0x7F000001"));
        }

    }
}
