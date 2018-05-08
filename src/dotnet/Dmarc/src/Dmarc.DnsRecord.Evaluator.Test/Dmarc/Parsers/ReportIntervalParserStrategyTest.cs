using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    public class ReportIntervalParserStrategyTest
    {
        private ReportIntervalParserStrategy _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new ReportIntervalParserStrategy();
        }

        [TestCase("86400", 86400, 0, TestName = "86400 valid report interval.")]
        [TestCase("3600", 3600, 0, TestName = "3600 valid report interval.")]
        [TestCase("-86400", 0, 1, TestName = "negative report interval invalid.")]
        [TestCase("asdf", 0, 1, TestName = "random string report interval invalid.")]
        [TestCase("", 0, 1, TestName = "emtpy string report interval invalid.")]
        [TestCase(null, 0, 1, TestName = "null report interval invalid.")]

        public void Test(string value, int reportInterval, int errorCount)
        {
            ReportInterval tag = (ReportInterval)_parser.Parse(string.Empty, value);

            Assert.That(tag.Interval, Is.EqualTo(errorCount == 0 ? (uint?)reportInterval : (uint?)null));
            Assert.That(tag.ErrorCount, Is.EqualTo(errorCount));
        }
    }
}