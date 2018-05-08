using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters.ReceivedHeader;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common.Converters.ReceivedHeader
{
    [TestFixture]
    public class ReceivedHeaderHostExtractorTests
    {
        private ReceivedHeaderHostExtractor _receivedHeaderHostExtractor;

        [SetUp]
        public void SetUp()
        {
            _receivedHeaderHostExtractor = new ReceivedHeaderHostExtractor(A.Fake<ILogger>());
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public void Test(string input, List<string> expected)
        {
            List<string> actual = _receivedHeaderHostExtractor.ExtractHosts(input);
            Assert.That(actual.SequenceEqual(expected), Is.True);
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            yield return new TestCaseData("abc.def.gov.uk", new List<string> { "abc.def.gov.uk" }).SetName("Single host correctly extracted");
            yield return new TestCaseData("abc.def.gov.uk (HELO def.ghi.gov.uk)", new List<string> { "abc.def.gov.uk", "def.ghi.gov.uk" }).SetName("Multiple hosts correctly extracted 1");
            yield return new TestCaseData("abc.def.gov.uk (HELO=def.ghi.gov.uk)", new List<string> { "abc.def.gov.uk", "def.ghi.gov.uk" }).SetName("Multiple hosts correctly extracted 2");
        }
    }
}
