using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    public class VersionParserStrategyTests
    {
        private VersionParserStrategy _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new VersionParserStrategy();
        }

        [TestCase("v=DMARC1", 0)]
        [TestCase("v=dmarc1", 0)]
        [TestCase("v=DMARC2", 1)]
        [TestCase("", 1)]
        [TestCase(null, 1)]
        public void Test(string value, int errorCount)
        {
            Evaluator.Dmarc.Domain.Version version = (Evaluator.Dmarc.Domain.Version)_parser.Parse(string.Empty, value);
            Assert.That(version.ErrorCount, Is.EqualTo(errorCount));
        }
    }
}
