using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Parsers
{
    [TestFixture]
    public class ModifierParserTests
    {
        private const string Redirect = "redirect";
        private const string Unknown = "unknown";
        private const string Domain = "a.b.com";

        private ModifierParser _parser;
        private IModifierParserStrategy _modifierParserStrategy;

        [SetUp]
        public void SetUp()
        {
            _modifierParserStrategy = FakeItEasy.A.Fake<IModifierParserStrategy>();
            
            FakeItEasy.A.CallTo(() => _modifierParserStrategy.Modifier).Returns(Redirect);
            _parser = new ModifierParser(new List<IModifierParserStrategy> {_modifierParserStrategy});
        }

        [Test]
        public void MatchingModifierAndStrategyReturnsTrue()
        {
            string modifier = $"{Redirect}={Domain}";
            Redirect redirect = new Redirect(modifier, new DomainSpec(Domain));
            FakeItEasy.A.CallTo(() => _modifierParserStrategy.Parse(modifier, Domain)).Returns(redirect);

            Term term;
            bool success = _parser.TryParse(modifier, out term);

            Assert.That(success, Is.True);
            Assert.That(term, Is.SameAs(redirect));

            FakeItEasy.A.CallTo(() => _modifierParserStrategy.Parse(modifier, Domain)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void MatchingModifierNoStrategyReturnsTrueWithUnknownModifier()
        {
            string modifier = $"{Unknown}={Domain}";
            
            Term term;
            bool success = _parser.TryParse(modifier, out term);

            Assert.That(success, Is.True);
            Assert.That(term, Is.TypeOf<UnknownModifier>());
            
            FakeItEasy.A.CallTo(() => _modifierParserStrategy.Parse(modifier, Domain)).MustNotHaveHappened();
        }

        [Test]
        public void ModifierDoesntMatchReturnFalse()
        {
            string modifier = $"{Redirect}&{Domain}";

            Term term;
            bool success = _parser.TryParse(modifier, out term);

            Assert.That(success, Is.False);
            Assert.That(term, Is.Null);
        }
    }
}