using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Rules;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Record
{
    [TestFixture]
    public class VersionMustBeFirstTagTests
    {
        private VersionMustBeFirstTag _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new VersionMustBeFirstTag();
        }

        [Test]
        public void VersionIsFirstTagNoError()
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag> { new Version("v=DMARC1") }, string.Empty, string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.False);

            Assert.That(error, Is.Null);
        }

        [Test]
        public void NoVersionTagError()
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag>(), string.Empty, string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.True);

            Assert.That(error, Is.Not.Null);
        }

        [Test]
        public void VersionTagIsNotFirstError()
        {
            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag>
            {
                new SubDomainPolicy("", PolicyType.None),
                new Version("v=DMARC1")
            }, string.Empty, string.Empty, false, false);

            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.True);

            Assert.That(error, Is.Not.Null);
        }
    }
}
