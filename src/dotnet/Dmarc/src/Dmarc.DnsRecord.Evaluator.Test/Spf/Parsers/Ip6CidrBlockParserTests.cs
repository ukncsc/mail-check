using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Parsers
{
    [TestFixture]
    public class Ip6CidrBlockParserTests
    {
        private Ip6CidrBlockParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new Ip6CidrBlockParser();
        }

        [TestCase("0", 0, 0, TestName = "0 valid ip6 cidr.")]
        [TestCase("128", 128, 0, TestName = "128 valid ip6 cidr.")]
        [TestCase("", 128, 0, TestName = "empty string valid ip46cidr.")]
        [TestCase(null, 128, 0, TestName = "null valid ip6 cidr.")]
        [TestCase("129", 0, 1, TestName = "129 invalid ip6 cidr.")]
        [TestCase("-1", 0, 1, TestName = "-1 invalid ip6 cidr.")]
        public void Test(string value, int cidr, int errorCount)
        {
            Ip6CidrBlock ip6CidrBlock = _parser.Parse(value);
            Assert.That(ip6CidrBlock.Value, Is.EqualTo(errorCount == 0 ? cidr : (int?)null));
            Assert.That(ip6CidrBlock.ErrorCount, Is.EqualTo(errorCount));
        }
    }
}