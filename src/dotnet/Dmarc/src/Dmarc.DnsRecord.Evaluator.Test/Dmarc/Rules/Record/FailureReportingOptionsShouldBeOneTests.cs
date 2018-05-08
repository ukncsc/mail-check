using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Rules;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Record
{
    [TestFixture]
    public class FailureReportingOptionsShouldBeOneTests
    {
        private FailureReportingOptionsShouldBeOne _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new FailureReportingOptionsShouldBeOne();
        }

        [TestCase(FailureOptionType.Unknown, true, TestName = "Error on unknown failure reporting option.")]
        [TestCase(FailureOptionType.D, true, TestName = "Error on d failure reporting option.")]
        [TestCase(FailureOptionType.S, true, TestName = "Error on s failure reporting option.")]
        [TestCase(FailureOptionType.Zero, true, TestName = "Error on 0 failure reporting option.")]
        [TestCase(FailureOptionType.One, false, TestName = "No Error on 1 failure reporting option.")]
        public void Test(FailureOptionType failureOptionType, bool isErrorExpected)
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag> { new FailureOption("", failureOptionType) }, string.Empty);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));

            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }

        public void NoErrorWhenFailureOptionTermNotFound()
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag>(), string.Empty);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.False);

            Assert.That(error, Is.Null);
        }
    }
}
