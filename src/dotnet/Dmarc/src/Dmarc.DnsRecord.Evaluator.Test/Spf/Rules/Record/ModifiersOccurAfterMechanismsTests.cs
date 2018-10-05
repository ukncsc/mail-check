using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Rules.Record;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Rules.Record
{
    [TestFixture]
    public class ModifiersOccurAfterMechanismsTests
    {
        private ModifiersOccurAfterMechanisms _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new ModifiersOccurAfterMechanisms();
        }

        [TestCaseSource(nameof(TestCaseSource))]
        public void Test(List<Term> terms, bool isErrorExpected)
        {
            SpfRecord spfRecord = new SpfRecord(string.Empty, new Version(string.Empty), terms, string.Empty);

            bool isErrored = _rule.IsErrored(spfRecord, out Evaluator.Rules.Error error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));
            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }

        private static IEnumerable<TestCaseData> TestCaseSource()
        {
            yield return new TestCaseData(new List<Term> { new All(string.Empty, Qualifier.Fail), new Redirect(string.Empty, new DomainSpec(string.Empty)) }, false).SetName("Mechanisms before modifiers no errors.");
            yield return new TestCaseData(new List<Term> { new Redirect(string.Empty, new DomainSpec(string.Empty)), new All(string.Empty, Qualifier.Fail) }, true).SetName("Modifiers before mechanisms errors.");
            yield return new TestCaseData(new List<Term> { new All(string.Empty, Qualifier.Fail) }, false).SetName("No modifiers no errors.");
            yield return new TestCaseData(new List<Term> { new Redirect(string.Empty, new DomainSpec(string.Empty)) }, false).SetName("No mechanisms no errors.");
            yield return new TestCaseData(new List<Term> { new Redirect(string.Empty, new DomainSpec(string.Empty)), new All(string.Empty, Qualifier.Fail, true) }, false).SetName("Implicit modifiers before mechanisms, no errors.");
        }
    }
}
