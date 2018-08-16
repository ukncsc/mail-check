using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    public class AspfParserStrategyTests
    {
        private AspfParserStrategy _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new AspfParserStrategy();
        }

        [TestCase("r", AlignmentType.R, 0)]
        [TestCase("s", AlignmentType.S, 0)]
        [TestCase("  s  ", AlignmentType.S, 0)]
        [TestCase("abc", AlignmentType.Unknown, 1)]
        [TestCase("0", AlignmentType.Unknown, 1)]
        [TestCase("25", AlignmentType.Unknown, 1)]
        [TestCase("2147483648", AlignmentType.Unknown, 1)]
        public void Test(string value, AlignmentType alignmentType, int errorCount)
        {
            string tagString = $"aspf={value}";
            Aspf tag = (Aspf)_parser.Parse(tagString, value);

            Assert.That(tag.Value, Is.EqualTo($"{tagString};"));
            Assert.That(tag.AlignmentType, Is.EqualTo(alignmentType));
            Assert.That(tag.ErrorCount, Is.EqualTo(errorCount));
        }
    }
}
