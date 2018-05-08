using System;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using Dmarc.DnsRecord.Evaluator.Rules;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    public class UriTagParserTests
    {
        private const string UriString = "mailto:a@b.com";
        private const string SizeString = "50m";

        private UriTagParser _parser;
        private IDmarcUriParser _dmarcUriParser;
        private IMaxReportSizeParser _maxReportSizeParser;

        [SetUp]
        public void SetUp()
        {
            _dmarcUriParser = A.Fake<IDmarcUriParser>();
            _maxReportSizeParser = A.Fake<IMaxReportSizeParser>();
            _parser = new UriTagParser(_dmarcUriParser, _maxReportSizeParser);   
        }

        [Test]
        public void UriAndMaxReportSizeTokensPresentFullyFormedUriTagReturned()
        {
            string value = $"{UriString}!{SizeString}";

            DmarcUri dmarcUri = new DmarcUri(new Uri(UriString));
            A.CallTo(() => _dmarcUriParser.Parse(UriString)).Returns(dmarcUri);

            MaxReportSize maxReportSize = new MaxReportSize(50, Unit.M);
            A.CallTo(() => _maxReportSizeParser.Parse(SizeString)).Returns(maxReportSize);

            UriTag uriTag = _parser.Parse(value);
            Assert.That(uriTag.Uri, Is.SameAs(dmarcUri));
            Assert.That(uriTag.MaxReportSize, Is.SameAs(maxReportSize));
            Assert.That(uriTag.ErrorCount, Is.Zero);

            A.CallTo(() => _dmarcUriParser.Parse(UriString)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _maxReportSizeParser.Parse(SizeString)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void OnlyUriTokenPresentUriWithNullMaxReportSizeReturned()
        {
            string value = $"{UriString}";

            DmarcUri dmarcUri = new DmarcUri(new Uri(UriString));
            A.CallTo(() => _dmarcUriParser.Parse(UriString)).Returns(dmarcUri);

            UriTag uriTag = _parser.Parse(value);
            Assert.That(uriTag.Uri, Is.SameAs(dmarcUri));
            Assert.That(uriTag.MaxReportSize, Is.Null);
            Assert.That(uriTag.ErrorCount, Is.Zero);

            A.CallTo(() => _dmarcUriParser.Parse(UriString)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _maxReportSizeParser.Parse(SizeString)).MustNotHaveHappened();
        }

        [Test]
        public void ExtraTokensFoundUriReturnedWithError()
        {
            string value = $"{UriString}!{SizeString}!{SizeString}";

            DmarcUri dmarcUri = new DmarcUri(new Uri(UriString));
            A.CallTo(() => _dmarcUriParser.Parse(UriString)).Returns(dmarcUri);

            MaxReportSize maxReportSize = new MaxReportSize(50, Unit.M);
            A.CallTo(() => _maxReportSizeParser.Parse(SizeString)).Returns(maxReportSize);

            UriTag uriTag = _parser.Parse(value);
            Assert.That(uriTag.Uri, Is.SameAs(dmarcUri));
            Assert.That(uriTag.MaxReportSize, Is.SameAs(maxReportSize));
            Assert.That(uriTag.ErrorCount, Is.EqualTo(1));

            A.CallTo(() => _dmarcUriParser.Parse(UriString)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _maxReportSizeParser.Parse(SizeString)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void EmptyStringProvidedUriReturnedWithError()
        {
            DmarcUri dmarcUri = new DmarcUri(null);
            dmarcUri.AddError(new Error(ErrorType.Error, string.Empty));
            A.CallTo(() => _dmarcUriParser.Parse(null)).Returns(dmarcUri);

            UriTag uriTag = _parser.Parse(string.Empty);
            Assert.That(uriTag.Uri, Is.SameAs(dmarcUri));
            Assert.That(uriTag.MaxReportSize, Is.Null);
            Assert.That(uriTag.AllErrorCount, Is.EqualTo(1));

            A.CallTo(() => _dmarcUriParser.Parse(null)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _maxReportSizeParser.Parse(SizeString)).MustNotHaveHappened();
        }

        [Test]
        public void NullStringProvidedUriReturnedWithError()
        {
            DmarcUri dmarcUri = new DmarcUri(null);
            dmarcUri.AddError(new Error(ErrorType.Error, string.Empty));
            A.CallTo(() => _dmarcUriParser.Parse(null)).Returns(dmarcUri);

            UriTag uriTag = _parser.Parse(null);
            Assert.That(uriTag.Uri, Is.SameAs(dmarcUri));
            Assert.That(uriTag.MaxReportSize, Is.Null);
            Assert.That(uriTag.AllErrorCount, Is.EqualTo(1));

            A.CallTo(() => _dmarcUriParser.Parse(null)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _maxReportSizeParser.Parse(SizeString)).MustNotHaveHappened();
        }
    }
}
