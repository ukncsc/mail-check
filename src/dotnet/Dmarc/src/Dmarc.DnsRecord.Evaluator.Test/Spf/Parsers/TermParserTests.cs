using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Parsers
{
    [TestFixture]
    public class TermParserTests
    {
        private TermParser _parser;
        private IMechanismParser _mechanismParser;
        private IModifierParser _modifierParser;

        [SetUp]
        public void SetUp()
        {
            _mechanismParser = FakeItEasy.A.Fake<IMechanismParser>();
            _modifierParser = FakeItEasy.A.Fake<IModifierParser>();
            _parser = new TermParser(_mechanismParser, _modifierParser);
        }

        [Test]
        public void ValidMechanismMechanismReturn()
        {
            string stringTerm = "term";

            Term term;
            Term expectedTerm = new All(stringTerm, Qualifier.Fail);
            FakeItEasy.A.CallTo(() => _mechanismParser.TryParse(stringTerm, out term))
                .Returns(true)
                .AssignsOutAndRefParameters(term = expectedTerm);

            Term actualTerm = _parser.Parse(stringTerm);

            Assert.That(actualTerm, Is.SameAs(expectedTerm));
            FakeItEasy.A.CallTo(() => _mechanismParser.TryParse(stringTerm, out term)).MustHaveHappened(Repeated.Exactly.Once);
            FakeItEasy.A.CallTo(() => _modifierParser.TryParse(stringTerm, out term)).MustNotHaveHappened();

        }

        [Test]
        public void ValidModifierModifierReturned()
        {
            string stringTerm = "term";


            Term term;
            FakeItEasy.A.CallTo(() => _mechanismParser.TryParse(stringTerm, out term))
                .Returns(false);

            Term expectedTerm = new Redirect(stringTerm, new DomainSpec(string.Empty));
            FakeItEasy.A.CallTo(() => _modifierParser.TryParse(stringTerm, out term))
                .Returns(true)
                .AssignsOutAndRefParameters(term = expectedTerm);

            Term actualTerm = _parser.Parse(stringTerm);

            Assert.That(actualTerm, Is.SameAs(expectedTerm));
            FakeItEasy.A.CallTo(() => _mechanismParser.TryParse(stringTerm, out term)).MustHaveHappened(Repeated.Exactly.Once);
            FakeItEasy.A.CallTo(() => _modifierParser.TryParse(stringTerm, out term)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void UnknownTermUnknownTermReturnedWithError()
        {
            string stringTerm = "term";

            Term term;
            FakeItEasy.A.CallTo(() => _mechanismParser.TryParse(stringTerm, out term))
                .Returns(false);

            FakeItEasy.A.CallTo(() => _modifierParser.TryParse(stringTerm, out term))
                .Returns(false);

            Term actualTerm = _parser.Parse(stringTerm);

            Assert.That(actualTerm, Is.TypeOf<UnknownTerm>());
            Assert.That(actualTerm.ErrorCount, Is.EqualTo(1));
            FakeItEasy.A.CallTo(() => _mechanismParser.TryParse(stringTerm, out term)).MustHaveHappened(Repeated.Exactly.Once);
            FakeItEasy.A.CallTo(() => _modifierParser.TryParse(stringTerm, out term)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
