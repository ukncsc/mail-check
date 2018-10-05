using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Rules.Record;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Rules.Record
{
    [TestFixture]
    public class DontUsePtrMechanismTest
    {
        private DontUsePtrMechanism _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new DontUsePtrMechanism();
        }

        [TestCase(false, false, TestName = "No ptr term no error.")]
        [TestCase(true, true, TestName = "Ptr term error.")]
        public void Test(bool ptrRecord, bool isErrorExpected)
        {
            Ptr ptr = ptrRecord
                ? new Ptr(string.Empty, Qualifier.Fail, new DomainSpec(string.Empty))
                : null;

            SpfRecord spfRecord = new SpfRecord("", new Version(""), new List<Term> { ptr }, string.Empty);

            bool isErrored = _rule.IsErrored(spfRecord, out Evaluator.Rules.Error error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));
            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }
    }
}
