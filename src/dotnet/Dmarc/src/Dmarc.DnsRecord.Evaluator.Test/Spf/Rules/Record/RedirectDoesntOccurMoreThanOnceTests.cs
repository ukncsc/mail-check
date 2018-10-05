using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Rules.Record;
using NUnit.Framework;
using Version = Dmarc.DnsRecord.Evaluator.Spf.Domain.Version;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Rules.Record
{
    [TestFixture]
    public class RedirectDoesntOccurMoreThanOnceTests
    {
        private RedirectDoesntOccurMoreThanOnce _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new RedirectDoesntOccurMoreThanOnce();
        }

        [TestCase(0, false, TestName = "No redirect term no error.")]
        [TestCase(1, false, TestName = "One redirect term no error.")]
        [TestCase(2, true, TestName = "Two redirect terms error.")]
        public void Test(int occurances, bool isErrorExpected)
        {
            List<Term> terms = Enumerable.Range(0, occurances)
                .Select(_ => new Redirect(string.Empty, new DomainSpec(string.Empty)))
                .Cast<Term>()
                .ToList();

            SpfRecord spfRecord = new SpfRecord(string.Empty, new Version(string.Empty), terms, string.Empty);

            bool isErrored = _rule.IsErrored(spfRecord, out Evaluator.Rules.Error error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));
            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }
    }
}
