using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Config;
using Dmarc.DnsRecord.Evaluator.Rules;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Config
{
    [TestFixture]
    public class OnlyOneDmarcRecordTests
    {
        private OnlyOneDmarcRecord _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new OnlyOneDmarcRecord();
        }

        [Test]
        public void WhenThereIsOnlyOneDmarcRecordNoErrorMessage()
        {
            DmarcConfig dmarcConfig =
                new DmarcConfig(new List<DmarcRecord> { new DmarcRecord("", new List<Tag>(), string.Empty) }, string.Empty, DateTime.UtcNow);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcConfig, out error);

            Assert.That(isErrored, Is.EqualTo(false));
            Assert.That(error, Is.Null);
        }

        [Test]
        public void WhenThereIsMoreThanOneDmarcRecordAErrorMessageIsReturned()
        {
            List<DmarcRecord> dmarcRecords = Enumerable.Range(0, 3).Select(_ => new DmarcRecord("", new List<Tag>(), string.Empty)).ToList();
            DmarcConfig dmarcConfig = new DmarcConfig(dmarcRecords, string.Empty, DateTime.UtcNow);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcConfig, out error);

            Assert.That(isErrored, Is.EqualTo(true));
            Assert.That(error.Message, Is.EqualTo(DmarcRulesResource.OnlyOneDmarcRecordErrorMessage));
        }

        [Test]
        public void WhenThereIsNoDmarcRecordAHelpfulMessageShouldBeReturned()
        {
            var domain = "abc.gov.uk";
            DmarcConfig dmarcConfig = new DmarcConfig(new List<DmarcRecord>(), domain, DateTime.UtcNow);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcConfig, out error);

            Assert.That(isErrored, Is.EqualTo(true));
            Assert.That(error.Message, Is.EqualTo(string.Format(DmarcRulesResource.NoDmarcErrorMessage, domain)));
            StringAssert.Contains(domain, error.Message);
        }
    }
}
