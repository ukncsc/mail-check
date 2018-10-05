using System.Collections.Generic;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Rules;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Record
{
    [TestFixture]
    public class SubDomainPolicyShouldBeQuarantineOrRejectTests
    {
        private SubDomainPolicyShouldBeQuarantineOrReject _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new SubDomainPolicyShouldBeQuarantineOrReject();
        }

        [TestCase(PolicyType.Unknown, false, "abc.com", TestName = "No error for unknown policy type on organisation domain.")]
        [TestCase(PolicyType.Quarantine, false, "abc.com", TestName = "No error for quarantine policy type on organisation domain.")]
        [TestCase(PolicyType.Reject, false, "abc.com", TestName = "No error for reject policy type on organisation domain.")]
        [TestCase(PolicyType.None, true, "abc.com", TestName = "Error for none policy type on organisation domain.")]
        [TestCase(PolicyType.Unknown, false, "xyz.abc.com", TestName = "No error for unknown policy type on non-organisation domain.")]
        [TestCase(PolicyType.Quarantine, false, "xyz.abc.com", TestName = "No error for quarantine policy type on non-organisation domain.")]
        [TestCase(PolicyType.Reject, false, "xyz.abc.com", TestName = "No error for reject policy type on non-organisation domain.")]
        [TestCase(PolicyType.None, false, "xyz.abc.com", TestName = "No error for none policy type on non-organisation domain.")]
        public void NoErrorWhenPolicyTermNotFound(PolicyType policyType, bool isErrorExpected, string domain)
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag> { new SubDomainPolicy("", policyType) }, domain, "abc.com",false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));

            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }

        [Test]
        public void NoErrorWhenPolicyTermNotFound()
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag>(), "abc.com", string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.False);

            Assert.That(error, Is.Null);
        }
    }
}
