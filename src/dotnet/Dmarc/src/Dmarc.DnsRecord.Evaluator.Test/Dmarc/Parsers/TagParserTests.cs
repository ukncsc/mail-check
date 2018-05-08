using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    public class TagParserTests
    {
        private const string TagName = "pct";

        private TagParser _parser;
        private ITagParserStrategy _strategy;

        [SetUp]
        public void SetUp()
        {
            _strategy = A.Fake<ITagParserStrategy>();
            A.CallTo(() => _strategy.Tag).Returns(TagName);
            _parser = new TagParser(new List<ITagParserStrategy> { _strategy });
        }

        [Test]
        public void MatchingStrategyTokensCorrectlyParsed()
        {
            string value = "100";
            string tagValue = $"{TagName}={value}";

            Percent percent = new Percent(tagValue, 100);

            A.CallTo(() => _strategy.Parse(tagValue, value)).Returns(percent);
            A.CallTo(() => _strategy.MaxOccurences).Returns(1);

            List<Tag> tags = _parser.Parse(new List<string> { tagValue });

            Assert.That(tags[0], Is.SameAs(percent));
            Assert.That(tags[0].ErrorCount, Is.Zero);
            A.CallTo(() => _strategy.Parse(tagValue, value)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void MatchingStrategyWithExtraTokensErrorAddedForExtraTokens()
        {
            string value = "100";
            string tagValue = $"{TagName}={value}={value}";

            Percent percent = new Percent(tagValue, 100);

            A.CallTo(() => _strategy.Parse(tagValue, value)).Returns(percent);
            A.CallTo(() => _strategy.MaxOccurences).Returns(1);

            List<Tag> tags = _parser.Parse(new List<string> { tagValue });

            Assert.That(tags[0], Is.SameAs(percent));
            Assert.That(tags[0].ErrorCount, Is.EqualTo(1));
            A.CallTo(() => _strategy.Parse(tagValue, value)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void NoMatchingStrategyUnknownTagCreatedWithError()
        {
            string value = "100";
            string tagValue = $"NotTagName={value}";

            List<Tag> tags = _parser.Parse(new List<string> { tagValue });

            Assert.That(tags[0], Is.TypeOf<UnknownTag>());
            Assert.That(tags[0].ErrorCount, Is.EqualTo(1));
            A.CallTo(() => _strategy.Parse(tagValue, value)).MustNotHaveHappened();
        }

        [Test]
        public void NullValueUnknownTagCreatedWithError()
        {
            List<Tag> tags = _parser.Parse(new List<string> { null });

            Assert.That(tags[0], Is.TypeOf<UnknownTag>());
            Assert.That(tags[0].ErrorCount, Is.EqualTo(1));
            A.CallTo(() => _strategy.Parse(A<string>._, A<string>._)).MustNotHaveHappened();
        }

        [Test]
        public void EmptyStringValueUnknownTagCreatedWithError()
        {
            List<Tag> tags = _parser.Parse(new List<string> { string.Empty });

            Assert.That(tags[0], Is.TypeOf<UnknownTag>());
            Assert.That(tags[0].ErrorCount, Is.EqualTo(1));
            A.CallTo(() => _strategy.Parse(A<string>._, A<string>._)).MustNotHaveHappened();
        }

        [Test]
        public void MaxOccurenceExceededTagCreatedWithError()
        {
            string value = "100";
            string tagValue = $"{TagName}={value}";

            Percent percent1 = new Percent(tagValue, 100);
            Percent percent2 = new Percent(tagValue, 100);

            A.CallTo(() => _strategy.Parse(tagValue, value)).ReturnsNextFromSequence(percent1, percent2);
            A.CallTo(() => _strategy.MaxOccurences).Returns(1);

            List<Tag> tags = _parser.Parse(new List<string> { tagValue, tagValue });

            Assert.That(tags[0], Is.SameAs(percent1));
            Assert.That(tags[0].ErrorCount, Is.Zero);

            Assert.That(tags[1], Is.SameAs(percent2));
            Assert.That(tags[1].ErrorCount, Is.EqualTo(1));
            Assert.That(tags[1].Errors[0].Message, Is.EqualTo("The pct tag should occur no more than once. This record has 2 occurrences."));
            A.CallTo(() => _strategy.Parse(tagValue, value)).MustHaveHappened(Repeated.Exactly.Twice);
        }
    }
}
