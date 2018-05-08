using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Parsers
{
    [TestFixture]
    public class Ip4CidrBlockParserTests
    {
        private Ip4CidrBlockParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new Ip4CidrBlockParser();
        }

        [TestCase("0", 0, 0, TestName = "0 valid ip4 cidr.")]
        [TestCase("32", 32, 0, TestName = "32 valid ip4 cidr.")]
        [TestCase("", 32, 0, TestName = "empty string valid ip4 cidr.")]
        [TestCase(null, 32, 0, TestName = "null valid ip4 cidr.")]
        [TestCase("33", 0, 1, TestName = "33 invalid ip4 cidr.")]
        [TestCase("-1", 0, 1, TestName = "-1 invalid ip4 cidr.")]
        public void Test(string value, int cidr, int errorCount)
        {
            Ip4CidrBlock ip4CidrBlock = _parser.Parse(value);
            Assert.That(ip4CidrBlock.Value, Is.EqualTo(errorCount == 0 ? cidr : (int?)null));
            Assert.That(ip4CidrBlock.ErrorCount, Is.EqualTo(errorCount));
        }
    }
}