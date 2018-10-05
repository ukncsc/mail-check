using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Rules;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Record
{
    [TestFixture]
    public class PolicyShouldBeQuarantineOrRejectTests
    {
        private PolicyShouldBeQuarantineOrReject _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new PolicyShouldBeQuarantineOrReject();
        }

        [TestCase(PolicyType.Unknown, false, TestName = "No error for unknown policy type.")]
        [TestCase(PolicyType.Quarantine, false, TestName = "No error for quarantine policy type.")]
        [TestCase(PolicyType.Reject, false, TestName = "No error for reject policy type.")]
        [TestCase(PolicyType.None, true, TestName = "Error for none policy type.")]
        public void Test(PolicyType policyType, bool isErrorExpected)
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag> { new Policy("", policyType) }, string.Empty, string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));

            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }

        [Test]
        public void NoErrorWhenPolicyTermNotFound()
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag>(), string.Empty, string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.False);

            Assert.That(error, Is.Null);
        }
    }
}
