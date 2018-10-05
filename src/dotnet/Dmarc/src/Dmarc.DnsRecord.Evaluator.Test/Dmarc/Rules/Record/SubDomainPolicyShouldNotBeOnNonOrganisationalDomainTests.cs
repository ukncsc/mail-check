using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Rules;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Rules.Record
{
    [TestFixture]
    public class SubDomainPolicyShouldNotBeOnNonOrganisationalDomainTests
    {
        private SubDomainPolicyShouldNotBeOnNonOrganisationalDomain _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new SubDomainPolicyShouldNotBeOnNonOrganisationalDomain();
        }

        [Test]
        public void NoErrorWhenOnOrganisationalDomain()
        {
            string domain = "abc.com";

            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag> { new SubDomainPolicy("", PolicyType.Unknown) }, domain, domain, false, false);
            
            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(false));

            Assert.That(error, Is.Null);
        }

        [Test]
        public void NoErrorWhenNosubDomainPolicyAndNonOrganisationalDomain()
        {
            string domain = "abc.com";

            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag>(), domain, domain, false, false);
            
            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(false));

            Assert.That(error, Is.Null);
        }

        [Test]
        public void NoErrorWhenOnNonOrganisationalDomainIsImpicit()
        {
            string domain = "abc.com";

            DmarcRecord dmarcRecord = new DmarcRecord("",
                new List<Tag> { new SubDomainPolicy("", PolicyType.Unknown, true) }, domain, "def.com", false, false);
            
            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(false));

            Assert.That(error, Is.Null);
        }

        [Test]
        public void ErrorWhenOnNonOrganisationalDomain()
        {
            string domain = "abc.com";

            DmarcRecord dmarcRecord = new DmarcRecord("", new List<Tag> { new SubDomainPolicy("", PolicyType.Unknown) }, domain, "def.com", false, false);
            
            Error error;
            bool isErrored = _rule.IsErrored(dmarcRecord, out error);

            Assert.That(isErrored, Is.EqualTo(true));

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo($"The specified sub-domain policy (sp) is ineffective because {domain} is not an organisational domain."));
        }

    }
}
