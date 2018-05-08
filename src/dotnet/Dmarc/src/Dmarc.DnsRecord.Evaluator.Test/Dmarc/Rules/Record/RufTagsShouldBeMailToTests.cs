using System;
using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Rules;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Record
{
    [TestFixture]
    public class RufTagsShouldBeMailToTests
    {
        private RufTagShouldBeMailTo _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new RufTagShouldBeMailTo();
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public void Test(Uri uri, bool isErrorExpected)
        {
            DmarcRecord dmarcRecord = new DmarcRecord("",
                new List<Tag> { new ReportUriForensic("", new List<UriTag> { new UriTag("", new DmarcUri(uri), new MaxReportSize(1000, Unit.K)) }) },
                string.Empty);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));

            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            //yield return new TestCaseData(null, false).SetName("No error when uri is null.");
            //yield return new TestCaseData(new Uri("mailto:a@b.com"), false).SetName("No error when uri is mailto.");
            yield return new TestCaseData(new Uri("http://a.c.com"), true).SetName("Error when uri is not mailto.");
        }

        [Test]
        public void NoErrorWhenRufTermNotFound()
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag>(), string.Empty);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.False);

            Assert.That(error, Is.Null);
        }
    }
}
