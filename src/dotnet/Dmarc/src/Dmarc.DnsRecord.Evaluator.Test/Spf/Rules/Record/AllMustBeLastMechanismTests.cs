using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Rules.Record;
using NUnit.Framework;
using Version = Dmarc.DnsRecord.Evaluator.Spf.Domain.Version;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Rules.Record
{
    [TestFixture]
    public class AllMustBeLastMechanismTests
    {
        private AllMustBeLastMechanism _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new AllMustBeLastMechanism();
        }

        [Test]
        public void NoAllMechanismNoError()
        {
            SpfRecord spfRecord = new SpfRecord(string.Empty, new Version(string.Empty), new List<Term>(), string.Empty);

            bool isErrored = _rule.IsErrored(spfRecord, out Evaluator.Rules.Error error);

            Assert.That(isErrored, Is.False);
            Assert.That(error, Is.Null);
        }

        [Test]
        public void AllIsLastMechanismNoError()
        {
            All all = new All(string.Empty, Qualifier.Fail);
            Include include = new Include(string.Empty, Qualifier.Pass, new DomainSpec(string.Empty));
            SpfRecord spfRecord = new SpfRecord("", new Version(""), new List<Term> { include, all }, string.Empty);

            bool isErrored = _rule.IsErrored(spfRecord, out Evaluator.Rules.Error error);
            Assert.That(isErrored, Is.False);
            Assert.That(error, Is.Null);
        }

        [Test]
        public void AllIsNotLastMechanismError()
        {
            Include include = new Include(string.Empty, Qualifier.Pass, new DomainSpec(string.Empty));
            All all = new All(string.Empty, Qualifier.Fail);
            SpfRecord spfRecord = new SpfRecord("", new Version(""), new List<Term> { all, include }, string.Empty);

            bool isErrored = _rule.IsErrored(spfRecord, out Evaluator.Rules.Error error);

            Assert.That(isErrored, Is.True);
            Assert.That(error, Is.Not.Null);
        }

        [Test]
        public void RedirectWithNoAllShouldHaveNoError()
        {
            Include include = new Include(string.Empty, Qualifier.Pass, new DomainSpec(string.Empty));
            All all = new All(string.Empty, Qualifier.Fail);
            Redirect redirect = new Redirect("ncsc.gov.uk", new DomainSpec("ncsc.gov.uk"));
            SpfRecord spfRecord = new SpfRecord("", new Version(""), new List<Term> { all, include, redirect }, string.Empty);

            bool isErrored = _rule.IsErrored(spfRecord, out Evaluator.Rules.Error error);

            Assert.That(isErrored, Is.False);
            Assert.That(error, Is.Null);
        }
    }
}
