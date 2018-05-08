using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using NUnit.Framework;
using Version = Dmarc.DnsRecord.Evaluator.Spf.Domain.Version;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Parsers
{
    [TestFixture]
    public class SpfVersionParserTests
    {
        private SpfVersionParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new SpfVersionParser();
        }

        [TestCase("v=spf1", 0, TestName = "valid version.")]
        [TestCase("v=SpF1", 0, TestName = "case insensitive.")]
        [TestCase("asdfasdf", 1, TestName = "random string invalid.")]
        [TestCase("", 1, TestName = "empty string invalid.")]
        [TestCase(null, 1, TestName = "null string invalid.")]
        public void Test(string value, int errorCount)
        {
            Version version = _parser.Parse(value);
            Assert.That(version.Value, Is.EqualTo(value));
            Assert.That(version.ErrorCount, Is.EqualTo(errorCount));
        }        
    }
}
