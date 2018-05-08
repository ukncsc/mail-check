using System;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Parsers
{
    [TestFixture]
    public class QualifierParserTests
    {
        private QualifierParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new QualifierParser();
        }

        [TestCase("", Qualifier.Pass)]
        [TestCase("+", Qualifier.Pass)]
        [TestCase("-", Qualifier.Fail)]
        [TestCase("?", Qualifier.Neutral)]
        [TestCase("~", Qualifier.SoftFail)]
        public void Test(string value, Qualifier expectedQualifier)
        {
            Qualifier qualifier = _parser.Parse(value);
            Assert.That(qualifier, Is.EqualTo(expectedQualifier));
        }

        [Test]
        public void ThrowsForUnknownQualifier()
        {
            Assert.Throws<ArgumentException>(() => _parser.Parse("asfd"));
        }
    }
}