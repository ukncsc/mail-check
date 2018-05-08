using System.Linq;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Parsers
{
    [TestFixture]
    public class Ip6AddrParserTests
    {
        private Ip6AddrParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new Ip6AddrParser();
        }

        [TestCase("fe80::b88a:c43a:2f51:89b6%8", 0, "", TestName = "Valid ip 6 address - valid.")]
        [TestCase("192.168.1.1", 1, "ipv6 address", TestName = "Valid ip 4 address - invalid.")]
        [TestCase("qwerqwer", 1, "ip address", TestName = "Invalid ip address - invalid.")]
        [TestCase("", 1, "ip address", TestName = "Empty string ip address - invalid.")]
        [TestCase(null, 1, "ip address", TestName = "Null ip address - invalid.")]
        public void Test(string ipString, int errorCount, string errorStringPattern)
        {
            Ip6Addr ip6Addr = _parser.Parse(ipString);

            Assert.That(ip6Addr.Value, Is.EqualTo(ipString));
            Assert.That(ip6Addr.ErrorCount, Is.EqualTo(errorCount));
            Assert.That((ip6Addr.Errors.FirstOrDefault()?.Message ?? string.Empty).Contains(errorStringPattern), Is.True);
        }
    }
}