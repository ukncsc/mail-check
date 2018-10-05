using System;
using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Rules;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Record
{
    [TestFixture]
    public class RuaTagsShouldBeMailToTests
    {
        private RuaTagsShouldBeMailTo _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new RuaTagsShouldBeMailTo();
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public void Test(string value, bool isErrorExpected)

        {
            Uri uri;

            Uri.TryCreate(value, UriKind.Absolute, out uri);

            DmarcRecord dmarcRecord = new DmarcRecord("",
                new List<Tag>
                {
                    new ReportUriAggregate("",
                        new List<UriTag>
                        {
                            new UriTag(value ?? "", new DmarcUri(uri),
                                new MaxReportSize(1000, Unit.K))
                        })
                },
                string.Empty, string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));

            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            yield return new TestCaseData(null, false).SetName("No error when uri is null.");
            yield return new TestCaseData("mailto:a@b.com", false).SetName("No error when uri is mailto.");
            yield return new TestCaseData("http://a.c.com", true).SetName("Error when uri is not mailto.");
            yield return new TestCaseData("a@c.com", true).SetName("Error when email provided without mailto.");
        }

        //[Test]
        //public void NoErrorWhenRuaTermNotFound()
        //{
        //    DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag>());

        //    Error error;
        //    bool isErrored = _rule.IsErrored(dmarcRecord, out error);

        //    Assert.That(isErrored, Is.False);

        //    Assert.That(error, Is.Null);
        //}
    }
}
