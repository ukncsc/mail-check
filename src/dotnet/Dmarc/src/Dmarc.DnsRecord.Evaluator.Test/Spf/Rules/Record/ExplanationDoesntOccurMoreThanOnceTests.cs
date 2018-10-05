using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Rules.Record;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Rules.Record
{
    [TestFixture]
    public class ExplanationDoesntOccurMoreThanOnceTests
    {
        private ExplanationDoesntOccurMoreThanOnce _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new ExplanationDoesntOccurMoreThanOnce();
        }

        [TestCase(0, false, TestName = "No exp term no error.")]
        [TestCase(1, false, TestName = "One exp term no error.")]
        [TestCase(2, true, TestName = "Exp term error.")]
        public void Test(int occurances, bool isErrorExpected)
        {
            List<Term> terms = Enumerable.Range(0, occurances)
                .Select(_ => new Explanation(string.Empty, new DomainSpec(string.Empty)))
                .Cast<Term>()
                .ToList();

            SpfRecord spfRecord = new SpfRecord(string.Empty, new Version(string.Empty), terms, string.Empty);

            bool isErrored = _rule.IsErrored(spfRecord, out Evaluator.Rules.Error error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));
            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }
    }
}
