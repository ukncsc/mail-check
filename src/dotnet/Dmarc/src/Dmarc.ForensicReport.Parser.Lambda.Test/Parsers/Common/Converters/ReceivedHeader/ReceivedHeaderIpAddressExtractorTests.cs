using System.Collections.Generic;
using System.Linq;
using System.Net;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters.ReceivedHeader;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common.Converters.ReceivedHeader
{
    [TestFixture]
    public class ReceivedHeaderIpAddressExtractorTests
    {
        private ReceivedHeaderIpAddressExtractor _receivedHeaderIpAddressExtractor;

        [SetUp]
        public void SetUp()
        {
            _receivedHeaderIpAddressExtractor = new ReceivedHeaderIpAddressExtractor(A.Fake<ILogger>());
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public void Test(string input, List<IPAddress> expected)
        {
            List<IPAddress> ipAddresses = _receivedHeaderIpAddressExtractor.ExtractIpAddresses(input);
            Assert.That(ipAddresses.SequenceEqual(expected));
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            yield return new TestCaseData("a-b.c.com (a-b.c.com [127.0.0.1])", new List<IPAddress> {IPAddress.Parse("127.0.0.1") }).SetName("Received header ip address correctly extracted from square brackets");
            yield return new TestCaseData("a-b.c.com (127.0.0.1)", new List<IPAddress> {IPAddress.Parse("127.0.0.1") }).SetName("Received header ip address correctly extracted from round brackets");
            yield return new TestCaseData("[127.0.0.1] (128.0.0.1)", new List<IPAddress> {IPAddress.Parse("127.0.0.1"), IPAddress.Parse("128.0.0.1") }).SetName("Received header multiple ip addresses extracted");
            yield return new TestCaseData("127.0.0.1", new List<IPAddress>()).SetName("Received header ip address without brackets not extracted");
        }
    }
}
