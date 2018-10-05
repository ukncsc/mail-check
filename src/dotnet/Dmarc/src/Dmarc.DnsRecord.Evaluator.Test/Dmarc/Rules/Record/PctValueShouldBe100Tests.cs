using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Rules;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Record
{
    [TestFixture]
    public class PctValueShouldBe100Tests
    {
        private PctValueShouldBe100 _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new PctValueShouldBe100();
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public void Test(int? percent, bool isErrorExpected)
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag> { new Percent("", percent) }, string.Empty, string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));

            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            yield return new TestCaseData(null, false).SetName("No error when pct value is null.");
            yield return new TestCaseData(100, false).SetName("No error when pct value is 100.");
            yield return new TestCaseData(99, true).SetName("Error when pct value is not 100.");
        }

        public void NoErrorWhenPercentTermNotFound()
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag>(), string.Empty, string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.False);

            Assert.That(error, Is.Null);
        }
    }
}
