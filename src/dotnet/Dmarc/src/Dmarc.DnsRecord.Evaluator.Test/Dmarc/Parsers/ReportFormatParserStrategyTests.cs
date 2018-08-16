using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    public class ReportFormatParserStrategyTests
    {
        private ReportFormatParserStrategy _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new ReportFormatParserStrategy();
        }

        [TestCase("AFRF", ReportFormatType.AFRF, 0, TestName = "AFRF valid report format.")]
        [TestCase("aFrF", ReportFormatType.AFRF, 0, TestName = "none valid report format.")]
        [TestCase("  AFRF  ", ReportFormatType.AFRF, 0, TestName = "none valid report format.")]
        [TestCase("asdf", ReportFormatType.Unknown, 1, TestName = "random string is invalid report format.")]
        [TestCase("0", ReportFormatType.Unknown, 1, TestName = "Zero report format is invalid.")]
        [TestCase("25", ReportFormatType.Unknown, 1, TestName = "Number report format is invalid.")]
        [TestCase("2147483648", ReportFormatType.Unknown, 1, TestName = "Large number report format is invalid.")]
        [TestCase("", ReportFormatType.Unknown, 1, TestName = "empty string is invalid report format.")]
        [TestCase(null, ReportFormatType.Unknown, 1, TestName = "null string is invalid report format.")]
        public void Test(string value, ReportFormatType reportFormatType, int errorCount)
        {
            ReportFormat tag = (ReportFormat)_parser.Parse(string.Empty, value);

            Assert.That(tag.ReportFormatType, Is.EqualTo(reportFormatType));
            Assert.That(tag.ErrorCount, Is.EqualTo(errorCount));
        }
    }
}
