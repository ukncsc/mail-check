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
        private IOrganisationalDomainProvider _organisationalDomainProvider;

        [SetUp]
        public void SetUp()
        {
            _organisationalDomainProvider = A.Fake<IOrganisationalDomainProvider>();
            _rule = new SubDomainPolicyShouldBeQuarantineOrReject(_organisationalDomainProvider);
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
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag> { new SubDomainPolicy("", policyType) }, domain);

            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain((domain)))
                .Returns(new OrganisationalDomain(domain, "abc.com"));

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));

            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }

        [Test]
        public void NoErrorWhenPolicyTermNotFound()
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag>(), "abc.com");

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.False);

            Assert.That(error, Is.Null);
        }
    }
}
