using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Rules.Config;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Rules.Config
{
    [TestFixture]
    public class OnlyOneSpfRecordTests
    {
        private OnlyOneSpfRecord _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new OnlyOneSpfRecord();
        }

        [TestCase(0, true)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public void Test(int count, bool isErrorExpected)
        {
            List<SpfRecord> spfRecords = Enumerable.Range(0, count).Select(_ => new SpfRecord(string.Empty, new Evaluator.Spf.Domain.Version(string.Empty), new List<Term>(), string.Empty)).ToList();
            SpfConfig spfConfig = new SpfConfig(spfRecords, DateTime.UtcNow);

            Error error;
            bool isErrored = _rule.IsErrored(spfConfig, out error);

            Assert.That(spfConfig.LastChecked.Date, Is.EqualTo(DateTime.UtcNow.Date));

            Assert.That(isErrored, Is.EqualTo(isErrorExpected));

            Assert.That(error, isErrorExpected ? Is.Not.Null : Is.Null);
        }
    }
}
