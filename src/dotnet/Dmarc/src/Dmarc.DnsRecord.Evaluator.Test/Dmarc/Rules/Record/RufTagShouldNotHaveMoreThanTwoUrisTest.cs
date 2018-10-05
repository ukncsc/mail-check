using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Rules;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Record
{
    [TestFixture]
    public class RufTagShouldNotHaveMoreThanTwoUrisTest
    {
        private RufTagShouldNotHaveMoreThanTwoUris _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new RufTagShouldNotHaveMoreThanTwoUris();
        }

        [TestCase(0, false, TestName = "No error when 0 Uris.")]
        [TestCase(1, false, TestName = "No error when 1 Uri.")]
        [TestCase(2, false, TestName = "No error when 2 Uris.")]
        [TestCase(3, true, TestName = "Error when 3 Uris.")]
        public void Test(int tagCount, bool isErrorExpected)
        {
            List<UriTag> uriTags = Enumerable.Range(0, tagCount)
                .Select(_ => new UriTag("", new DmarcUri(new Uri("mailto:a@b.com")), new MaxReportSize(1000, Unit.K))).ToList();

            ReportUriForensic reportUriForensic = new ReportUriForensic("", uriTags);

            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag> { reportUriForensic }, string.Empty, string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));

            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }

        [Test]
        public void NoErrorWhenRuaTermNotFound()
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag>(), string.Empty, string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.False);

            Assert.That(error, Is.Null);
        }
    }
}
