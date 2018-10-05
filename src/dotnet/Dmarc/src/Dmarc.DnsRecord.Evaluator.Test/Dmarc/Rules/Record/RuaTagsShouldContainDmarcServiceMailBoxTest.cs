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
    public class RuaTagsShouldContainDmarcServiceMailBoxTest
    {
        private RuaTagsShouldContainDmarcServiceMailBox _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new RuaTagsShouldContainDmarcServiceMailBox();
        }

        public void Test(DmarcRecord dmarcRecord, bool isErrorExpected, ErrorType? expectedError = null)
        {
            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));

            Assert.That(error?.ErrorType, Is.EqualTo(expectedError));
        }

        [Test]
        public void ErrorWhenIncorrectMailCheckMailbox()
        {
            var testDmarcRecord = CreateDmarcRecord(
                CreateReportUriAggregate(new Uri("mailto:rua@dmarc.service.gov.uk")));

            Test(testDmarcRecord, true, ErrorType.Error);
        }

        [Test]
        public void NoErrorWhenDuplicateTags()
        {
            var testDmarcRecord = CreateDmarcRecord(
                CreateReportUriAggregate(new Uri("mailto:dmarc-rua@dmarc.service.gov.uk")),
                CreateReportUriAggregate(new Uri("mailto:blah@somewhere.co.uk")));

            Test(testDmarcRecord, false);
        }

        [Test]
        public void WarningWhenSameMailboxMentionedMoreThanOnce()
        {
            var testDmarcRecord = CreateDmarcRecord(
                CreateReportUriAggregate(
                    new Uri("mailto:a@b.com"),
                    new Uri("mailto:a@b.com"),
                    new Uri("mailto:dmarc-rua@dmarc.service.gov.uk")));

            Test(testDmarcRecord, true, ErrorType.Warning);
        }

        [Test]
        public void ErrorWhenMailCheckRufMailboxUsed()
        {
            var testDmarcRecord = CreateDmarcRecord(
                CreateReportUriAggregate(new Uri("mailto:dmarc-ruf@dmarc.service.gov.uk")));

            Test(testDmarcRecord, true, ErrorType.Error);
        }

        [Test]
        public void WarningWhenNoMailCheckMailbox()
        {
            var testDmarcRecord = CreateDmarcRecord(
                CreateReportUriAggregate(new Uri("mailto:a@b.com")));

            Test(testDmarcRecord, true, ErrorType.Warning);
        }

        [Test]
        public void NoErrorWhenCorrectMailboxIsUsed()
        {
            var testDmarc = CreateDmarcRecord(
                CreateReportUriAggregate(new Uri("mailto:dmarc-rua@dmarc.service.gov.uk")));

            Test(testDmarc, false);
        }

        [Test]
        public void WarningWhenNoUris()
        {
            var testDmarc = CreateDmarcRecord(CreateReportUriAggregate());

            Test(testDmarc, true, ErrorType.Warning);
        }

        [Test]
        public void NoExceptionWhenNullUri()
        {
            var testDmarc = CreateDmarcRecord(
                CreateReportUriAggregate(new[] { new Uri("mailto:dmarc-rua@dmarc.service.gov.uk"), null }));

            Assert.DoesNotThrow(() => Test(testDmarc, false));
        }

        private static DmarcRecord CreateDmarcRecord(params Tag[] tags)
        {
            return new DmarcRecord("", tags.ToList(), string.Empty, string.Empty, false, false);
        }

        private static ReportUriAggregate CreateReportUriAggregate(params Uri[] uris)
        {
            return new ReportUriAggregate("", uris?.Select(_ => new UriTag("", new DmarcUri(_), new MaxReportSize(1000, Unit.K))).ToList());
        }
    }
}
