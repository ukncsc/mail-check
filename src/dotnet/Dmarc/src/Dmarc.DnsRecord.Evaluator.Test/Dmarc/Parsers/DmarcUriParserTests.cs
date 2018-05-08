using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    public class DmarcUriParserTests
    {
        private DmarcUriParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new DmarcUriParser();
        }

        [TestCase("mailto:a@b.com", 0)]
        [TestCase("http://b.com/", 0)]
        [TestCase("ftp://b.com/", 0)]
        [TestCase("a@b.com", 1)]
        [TestCase("asdfasdf", 1)]
        [TestCase("", 1)]
        [TestCase(null, 1)]
        public void Test(string uri, int errorCount)
        {
            DmarcUri dmarcUri = _parser.Parse(uri);

            Assert.That(dmarcUri.Uri?.ToString(), Is.EqualTo(errorCount == 0 ? uri : null));
            Assert.That(dmarcUri.ErrorCount, Is.EqualTo(errorCount));
        }
    }
}
