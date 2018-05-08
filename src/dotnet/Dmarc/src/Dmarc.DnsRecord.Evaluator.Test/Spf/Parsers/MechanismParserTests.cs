using System;
using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Parsers
{
    [TestFixture]
    public class MechanismParserTests
    {
        private const string PassQualifier = "+";
        private const string IncludeMechanism = "include";
        private const string Domain = "a.b.com";

        private MechanismParser _parser;
        private IQualifierParser _qualifierParser;
        private IMechanismParserStrategy _mechanismParserStrategy;

        [SetUp]
        public void SetUp()
        {
            _qualifierParser = FakeItEasy.A.Fake<IQualifierParser>();
            _mechanismParserStrategy = FakeItEasy.A.Fake<IMechanismParserStrategy>();
            FakeItEasy.A.CallTo(() => _mechanismParserStrategy.Mechanism).Returns(IncludeMechanism);
            _parser = new MechanismParser(_qualifierParser, new List<IMechanismParserStrategy> {_mechanismParserStrategy});
        }

        [Test]
        public void MatchingMechanismAndStrategyReturnsTrue()
        {
            string mechanism = $"{PassQualifier}{IncludeMechanism}:{Domain}";

            Include include = new Include(mechanism, Qualifier.Pass, new DomainSpec(Domain));
            FakeItEasy.A.CallTo(() => _mechanismParserStrategy.Parse(mechanism, Qualifier.Pass, Domain)).Returns(include);

            Term term;
            bool success = _parser.TryParse(mechanism, out term);

            Assert.That(success, Is.True);
            Assert.That(term, Is.SameAs(include));
            FakeItEasy.A.CallTo(() => _qualifierParser.Parse(PassQualifier)).MustHaveHappened(Repeated.Exactly.Once);
            FakeItEasy.A.CallTo(() => _mechanismParserStrategy.Parse(mechanism, Qualifier.Pass, Domain)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void MatchMechanismNoStrategyThrows()
        {
            Term term;
            Assert.Throws<ArgumentException>(() => _parser.TryParse("+a", out term));
        }

        [Test]
        public void MechanismDoesntMatchAndStrategyReturnsFalse()
        {
            string mechanism = $"*{IncludeMechanism}:{Domain}";

            Include include = new Include(mechanism, Qualifier.Pass, new DomainSpec(Domain));
            FakeItEasy.A.CallTo(() => _mechanismParserStrategy.Parse(mechanism, Qualifier.Pass, Domain)).Returns(include);

            Term term;
            bool success = _parser.TryParse(mechanism, out term);

            Assert.That(success, Is.False);
            Assert.That(term, Is.Null);
            FakeItEasy.A.CallTo(() => _qualifierParser.Parse(PassQualifier)).MustNotHaveHappened();
            FakeItEasy.A.CallTo(() => _mechanismParserStrategy.Parse(mechanism, Qualifier.Pass, Domain)).MustNotHaveHappened();
        }
    }
}