using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Parsers
{
    [TestFixture]
    public class DomainSpecParserPassiveTests
    {
        private DomainSpecParserPassive _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new DomainSpecParserPassive();
        }

        [TestCase("a.b.com", true, 0, TestName = "valid domain.")]
        [TestCase("_spf.xyz.com", true, 0, TestName = "valid domain.")]
        [TestCase("spf_c.abc.com", true, 0, TestName = "valid domain.")]
        [TestCase("%{d}.d.spf.example.com", true, 0, TestName = "valid macro.")]
        [TestCase("%{1}.d.spf.example.com", true, 1, TestName = "invalid macro.")]
        [TestCase("", true, 1, TestName = "empty string domain spec.")]
        [TestCase(null, true, 1, TestName = "null domain spec.")]
        public void Test(string value, bool mandatory, int errorCount)
        {
            DomainSpec domainSpec = _parser.Parse(value, mandatory);
            Assert.That(domainSpec.Domain, Is.EqualTo(value));
            Assert.That(domainSpec.ErrorCount, Is.EqualTo(errorCount));
        }
    }
}