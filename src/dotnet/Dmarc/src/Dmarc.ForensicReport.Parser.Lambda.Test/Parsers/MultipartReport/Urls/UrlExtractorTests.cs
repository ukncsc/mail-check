using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Urls;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.Urls
{
    [TestFixture]
    public class UrlExtractorTests
    {
        private UrlExtractor _urlExtractor;

        [SetUp]
        public void SetUp()
        {
            _urlExtractor = new UrlExtractor(A.Fake<ILogger>());   
        }

        [TestCaseSource(nameof(CreateOtherTestData))]
        public void OtherTests(string input, List<string> expected)
        {
            List<string> results = _urlExtractor.ExtractUrls(input);
            Assert.True(results.SequenceEqual(expected));
        }

        public static IEnumerable<TestCaseData> CreateOtherTestData()
        {
            yield return new TestCaseData("http://foo.com/blah", new List<string> { "http://foo.com/blah" }).SetName("Extracts url");
            yield return new TestCaseData("http://127.0.0.1/blah", new List<string> { "http://127.0.0.1/blah" }).SetName("Extracts url with ip");
            yield return new TestCaseData("http://foo.com/blah?id=123", new List<string> { "http://foo.com/blah?id=123" }).SetName("Extracts url with params");
            yield return new TestCaseData("http://foo.com/#/blah", new List<string> { "http://foo.com/#/blah" }).SetName("Extracts url with hash path");
            yield return new TestCaseData("http://foo.com/#/blah?id=123", new List<string> { "http://foo.com/#/blah?id=123" }).SetName("Extracts url with hash path and params");
            yield return new TestCaseData("<a href=\"http://foo.com/blah\">Link text</a>", new List<string> { "http://foo.com/blah" }).SetName("Extracts url from anchor tag");
            yield return new TestCaseData("<img src=\"http://foo.com/blah/image.jpg\" alt=\"Image\" height=\"100\" width=\"100\">", new List<string> { "http://foo.com/blah/image.jpg" }).SetName("Extracts url from img tag");
            yield return new TestCaseData("http://bit.ly/2mICfiH", new List<string> { "http://bit.ly/2mICfiH" }).SetName("Shortened urls extracted correcltly");
            yield return new TestCaseData("http://foo.com/blah http://foo.com/blah2", new List<string> { "http://foo.com/blah", "http://foo.com/blah2" }).SetName("Multiple urls correctly extracted");
            yield return new TestCaseData("http://foo.com/blah http://foo.com/blah", new List<string> { "http://foo.com/blah" }).SetName("Urls deduplicated");
        }
    }
}
