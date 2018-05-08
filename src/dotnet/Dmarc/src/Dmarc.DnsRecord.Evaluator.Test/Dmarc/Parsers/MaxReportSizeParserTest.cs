using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    public class MaxReportSizeParserTest
    {
        private MaxReportSizeParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new MaxReportSizeParser();
        }

        [TestCaseSource(nameof(TestCaseSource))]
        public void Test(string value, ulong? size, Unit unit, int errorCount)
        {
            MaxReportSize maxReportSize = _parser.Parse(value);

            Assert.That(maxReportSize.Value, Is.EqualTo(size));
            Assert.That(maxReportSize.Unit, Is.EqualTo(unit));
            Assert.That(maxReportSize.ErrorCount, Is.EqualTo(errorCount));
        }

        public static IEnumerable<TestCaseData> TestCaseSource()
        {
            yield return new TestCaseData("100k", (ulong?)100, Unit.K, 0).SetName("100k is valid max size for reports.");
            yield return new TestCaseData("1g", (ulong?)1, Unit.G, 0).SetName("1g is valid max size for reports.");
            yield return new TestCaseData("10m", (ulong?)10, Unit.M, 0).SetName("10m is valid max size for reports.");
            yield return new TestCaseData("1t", (ulong?)1, Unit.T, 0).SetName("1t is valid max size for reports.");
            yield return new TestCaseData("10000", (ulong?)10000, Unit.B, 0).SetName("Bytes is default unit.");
            yield return new TestCaseData(string.Empty, (ulong?)null, Unit.Unknown, 1).SetName("Error if value is empty.");
            yield return new TestCaseData(null, (ulong?)null, Unit.Unknown, 1).SetName("Error if value is null.");
            yield return new TestCaseData("-1t", (ulong?)null, Unit.Unknown, 1).SetName("Error if size is negative.");
            yield return new TestCaseData("18446744073709551616", (ulong?)null, Unit.B, 1).SetName("Error if size larger than ulong.MaxValue.");
        }
    }
}