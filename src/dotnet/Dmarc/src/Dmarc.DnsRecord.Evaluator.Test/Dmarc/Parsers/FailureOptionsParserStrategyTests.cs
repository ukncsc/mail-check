using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    public class FailureOptionsParserStrategyTests
    {
        private FailureOptionsParserStrategy _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new FailureOptionsParserStrategy();
        }

        [TestCase("0", FailureOptionType.Zero, 0)]
        [TestCase("1", FailureOptionType.One, 0)]
        [TestCase("s", FailureOptionType.S, 0)]
        [TestCase("d", FailureOptionType.D, 0)]
        [TestCase("S", FailureOptionType.S, 0)]
        [TestCase("D", FailureOptionType.D, 0)]
        [TestCase("asdfa", FailureOptionType.Unknown, 1)]
        [TestCase("", FailureOptionType.Unknown, 1)]
        [TestCase(null, FailureOptionType.Unknown, 1)]
        public void Test(string value, FailureOptionType failureOptionType, int errorCount)
        {
            FailureOption tag = (FailureOption)_parser.Parse(string.Empty, value);

            Assert.That(tag.FailureOptionType, Is.EqualTo(failureOptionType));
            Assert.That(tag.ErrorCount, Is.EqualTo(errorCount));
        }
    }
}