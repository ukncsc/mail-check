using System;
using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Config;
using Dmarc.DnsRecord.Evaluator.Rules;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Config
{
    [TestFixture]
    public class TldDmarcRecordBehaviourIsWeaklyDefinedTests
    {
        private TldDmarcRecordBehaviourIsWeaklyDefined sut;

        [SetUp]
        public void SetUp()
        {
            sut = new TldDmarcRecordBehaviourIsWeaklyDefined();
        }

        [Test]
        public void ATldWithADmarcRecordShouldFail()
        {
            Assert.That(sut.IsErrored(CreateConfig("gov.uk", true, true), out Error error), Is.True);
            Assert.That(error, Is.Not.Null);
            Assert.AreEqual(error.ErrorType, ErrorType.Warning);
        }

        [Test]
        public void ATldWithNoDmarcRecordShouldPass()
        {
            Assert.That(sut.IsErrored(CreateConfig("gov.uk", false, true), out Error error), Is.False);
            Assert.That(error, Is.Null);
        }

        [Test]
        public void ANonTldWithADmarcRecordShouldPass()
        {
            Assert.That(sut.IsErrored(CreateConfig("ncsc.gov.uk", true), out Error error), Is.False);
            Assert.That(error, Is.Null);
        }

        [Test]
        public void ANonTldWithNoDmarcRecordShouldPass()
        {
            Assert.That(sut.IsErrored(CreateConfig("ncsc.gov.uk"), out Error error), Is.False);
            Assert.That(error, Is.Null);
        }

        private static DmarcConfig CreateConfig(string domain, bool hasRecord = false, bool isTld = false, bool isInherited = false)
        {
            List<DmarcRecord> records = hasRecord
                ? new List<DmarcRecord>() { new DmarcRecord("v=DMARC1;", A.Fake<List<Tag>>(), domain, domain, isTld, isInherited) }
                : new List<DmarcRecord>();

            return new DmarcConfig(records, domain, DateTime.Now, domain, isTld, isInherited);
        }
    }
}
