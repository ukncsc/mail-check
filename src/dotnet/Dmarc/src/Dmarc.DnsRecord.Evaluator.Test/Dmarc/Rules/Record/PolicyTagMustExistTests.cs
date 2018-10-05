using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Rules;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Record
{
    [TestFixture]
    public class PolicyTagMustExistTests
    {
        private PolicyTagMustExist _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new PolicyTagMustExist();
        }

        [TestCase(true, false, TestName = "No error if policy tag exists.")]
        [TestCase(false, true, TestName = "Error if policy tag doesnt exists.")]
        public void Test(bool policyTagExists, bool isErrorExpected)
        {
            Policy policy = policyTagExists ? new Policy("", PolicyType.None) : null;
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag> { policy }, string.Empty, string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));

            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }
    }
}
