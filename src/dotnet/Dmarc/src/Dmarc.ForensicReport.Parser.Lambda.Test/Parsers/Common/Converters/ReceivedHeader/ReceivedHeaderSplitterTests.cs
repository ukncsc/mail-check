using System.Collections.Generic;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters.ReceivedHeader;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common.Converters.ReceivedHeader
{
    [TestFixture]
    public class ReceivedHeaderSplitterTests
    {
        private ReceivedHeaderSplitter _receivedHeaderSplitter;

        [SetUp]
        public void SetUp()
        {
            _receivedHeaderSplitter = new ReceivedHeaderSplitter(A.Fake<ILogger>());
        }

        [Test]
        public void ReceivedHeaderCorrectlySplit()
        {
            string receivedField ="from a.b.c by d.e.f via g.h.i with SMTP id abc for a.b@gov.uk;\tTue, 1 Nov 2016 04:57:41 - 0700";
            List<string> values = _receivedHeaderSplitter.Split(receivedField);
            Assert.That(values.Count, Is.EqualTo(14));
            Assert.That(values[0], Is.EqualTo("from"));
            Assert.That(values[1], Is.EqualTo("a.b.c"));
            Assert.That(values[2], Is.EqualTo("by"));
            Assert.That(values[3], Is.EqualTo("d.e.f"));
            Assert.That(values[4], Is.EqualTo("via"));
            Assert.That(values[5], Is.EqualTo("g.h.i"));
            Assert.That(values[6], Is.EqualTo("with"));
            Assert.That(values[7], Is.EqualTo("smtp"));
            Assert.That(values[8], Is.EqualTo("id"));
            Assert.That(values[9], Is.EqualTo("abc"));
            Assert.That(values[10], Is.EqualTo("for"));
            Assert.That(values[11], Is.EqualTo("a.b@gov.uk"));
            Assert.That(values[12], Is.EqualTo(";"));
            Assert.That(values[13], Is.EqualTo("tue, 1 nov 2016 04:57:41 - 0700"));
        }
    }
}
