using System;
using System.Collections.Generic;
using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters.ReceivedHeader;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.Rfc822.Headers
{
    [TestFixture]
    public class ReceivedParserMultiTests
    {
        private ReceivedHeaderParserMulti _receivedHeaderParserMulti;
        private IReceivedHeaderConverter _receivedHeaderConverter;

        [SetUp]
        public void SetUp()
        {
            _receivedHeaderConverter = A.Fake<IReceivedHeaderConverter>();
            _receivedHeaderParserMulti = new ReceivedHeaderParserMulti(_receivedHeaderConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "from x.y.z by a.c.d" } } };


            ReceivedHeader receivedHeader = new ReceivedHeader("from a.b.c", "by d.e.f", new MailAddress("a.b@gov.uk"));
            A.CallTo(() => _receivedHeaderConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(receivedHeader);

            List<ReceivedHeader> receivedHeaders = _receivedHeaderParserMulti.Parse(headers, "header1", false, false, false);

            Assert.That(receivedHeaders.Count, Is.EqualTo(1));
            Assert.That(receivedHeaders[0], Is.EqualTo(receivedHeader));
            A.CallTo(() => _receivedHeaderConverter.Convert(A<string>._, A<string>._, A<bool>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void MultipleValuesAllowed()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "from x.y.z by a.c.d", "from x.y.z by a.c.d" } } };


            ReceivedHeader receivedHeader1 = new ReceivedHeader("from a.b.c", "by d.e.f", new MailAddress("a.b@gov.uk"));
            ReceivedHeader receivedHeader2 = new ReceivedHeader("from a.b.c", "by d.e.f", new MailAddress("a.b@gov.uk"));
            A.CallTo(() => _receivedHeaderConverter.Convert(A<string>._, A<string>._, A<bool>._)).ReturnsNextFromSequence(receivedHeader1, receivedHeader2);

            List<ReceivedHeader> receivedHeaders = _receivedHeaderParserMulti.Parse(headers, "header1", false, false, false);

            Assert.That(receivedHeaders.Count, Is.EqualTo(2));
            Assert.That(receivedHeaders[0], Is.EqualTo(receivedHeader1));
            Assert.That(receivedHeaders[1], Is.EqualTo(receivedHeader2));
            A.CallTo(() => _receivedHeaderConverter.Convert(A<string>._, A<string>._, A<bool>._)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void FieldDoenstExistReturnsEmptyList()
        {
            List<ReceivedHeader> receivedHeaders = _receivedHeaderParserMulti.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(receivedHeaders, Is.Empty);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _receivedHeaderParserMulti.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<ArgumentException>(() => _receivedHeaderParserMulti.Parse(headers, "header1", false, true, false));
        }
    }
}
