using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    public class PercentParserStrategyTests
    {
        private PercentParserStrategy _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new PercentParserStrategy();
        }

        [TestCase("0", 0, 0, TestName = "0 valid percentage.")]
        [TestCase("100", 100, 0, TestName = "100 valid percentage.")]
        [TestCase("-1", 0, 1, TestName = "-1 invalid percentage.")]
        [TestCase("101", 0, 1, TestName = "101 invalid percentage.")]
        [TestCase("", 0, 1, TestName = "empty string invalid percentage.")]
        [TestCase(null, 0, 1, TestName = "null invalid percentage.")]
        public void Test(string value, int pct, int errorCount)
        {
            Percent tag = (Percent)_parser.Parse(string.Empty, value);
            
            Assert.That(tag.PercentValue, Is.EqualTo(errorCount == 0 ? (int?)pct : (int?)null));
            Assert.That(tag.ErrorCount, Is.EqualTo(errorCount));
        }
    }
}