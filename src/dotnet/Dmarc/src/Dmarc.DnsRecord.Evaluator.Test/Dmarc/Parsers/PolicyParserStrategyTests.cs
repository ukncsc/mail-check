using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    public class PolicyParserStrategyTests
    {
        private PolicyParserStrategy _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new PolicyParserStrategy();
        }

        [TestCase("none", PolicyType.None, 0, TestName = "none valid policy.")]
        [TestCase("quarantine", PolicyType.Quarantine, 0, TestName = "quarantine valid policy.")]
        [TestCase("reject", PolicyType.Reject, 0, TestName = "reject valid policy,")]
        [TestCase("NoNe", PolicyType.None, 0, TestName = "policy parsing is case insensitive.")]
        [TestCase("  none  ", PolicyType.None, 0, TestName = "policy parsing ignores whitespace.")]
        [TestCase("reject123", PolicyType.Unknown, 1, TestName = "random string is invalid policy.")]
        [TestCase("0", PolicyType.Unknown, 1, TestName = "Zero policy is invalid.")]
        [TestCase("25", PolicyType.Unknown, 1, TestName = "Number policy is invalid.")]
        [TestCase("2147483648", PolicyType.Unknown, 1, TestName = "Large number policy is invalid.")]
        [TestCase("", PolicyType.Unknown, 1, TestName = "Empty policy is invalid.")]
        [TestCase(null, PolicyType.Unknown, 1, TestName = "Null policy is invalid.")]
        public void Test(string value, PolicyType policyType, int errorCount)
        {
            Policy tag = (Policy)_parser.Parse(string.Empty, value);

            Assert.That(tag.PolicyType, Is.EqualTo(policyType));
            Assert.That(tag.ErrorCount, Is.EqualTo(errorCount));
        }
    }

    [TestFixture]
    public class SubDomainPolicyParserStrategyTests
    {
        private SubDomainPolicyParserStrategy _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new SubDomainPolicyParserStrategy();
        }

        [TestCase("none", PolicyType.None, 0, TestName = "none valid sub domain policy.")]
        [TestCase("quarantine", PolicyType.Quarantine, 0, TestName = "quarantine valid sub domain policy.")]
        [TestCase("reject", PolicyType.Reject, 0, TestName = "reject valid sub domain policy,")]
        [TestCase("NoNe", PolicyType.None, 0, TestName = "sub domain policy parsing is case insensitive.")]
        [TestCase("  none  ", PolicyType.None, 0, TestName = "sub domain policy parsing ignores whitespace.")]
        [TestCase("reject123", PolicyType.Unknown, 1, TestName = "random string is invalid sub domain policy.")]
        [TestCase("0", PolicyType.Unknown, 1, TestName = "Zero sub domain policy is invalid.")]
        [TestCase("25", PolicyType.Unknown, 1, TestName = "Number sub domain policy is invalid.")]
        [TestCase("2147483648", PolicyType.Unknown, 1, TestName = "Large number sub domain policy is invalid.")]
        [TestCase("", PolicyType.Unknown, 1, TestName = "Empty sub domain policy is invalid.")]
        [TestCase(null, PolicyType.Unknown, 1, TestName = "Null sub domain policy is invalid.")]
        public void Test(string value, PolicyType policyType, int errorCount)
        {
            SubDomainPolicy tag = (SubDomainPolicy)_parser.Parse(string.Empty, value);

            Assert.That(tag.PolicyType, Is.EqualTo(policyType));
            Assert.That(tag.ErrorCount, Is.EqualTo(errorCount));
        }
    }
}
