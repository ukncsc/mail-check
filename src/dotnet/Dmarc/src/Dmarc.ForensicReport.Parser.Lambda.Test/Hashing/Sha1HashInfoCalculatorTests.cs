using System.Collections.Generic;
using System.IO;
using System.Text;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Hashing;
using MimeKit;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Hashing
{
    [TestFixture]
    public class Sha1HashInfoCalculatorTests
    {
        private Sha1HashInfoCalculator _sha1HashInfoCalculator;

        [SetUp]
        public void SetUp()
        {
            _sha1HashInfoCalculator = new Sha1HashInfoCalculator();
        }

        [TestCaseSource(nameof(CreateTestData))]
        public void Test(MimePart mimePart, string expectedHashValue)
        {
            HashInfo hashInfo = _sha1HashInfoCalculator.Calculate(mimePart);
            Assert.That(hashInfo.Hash, Is.EqualTo(expectedHashValue));
        }

        public static IEnumerable<TestCaseData> CreateTestData()
        {
            yield return new TestCaseData(CreateMimePart("text/html", "<!DOCTYPE html><html><head><title>Title</title></head><body></body></html>"), "752b0b23d0ad9e35da23959e0179de62d53156b3");
            yield return new TestCaseData(CreateMimePart("text/html", ""), "da39a3ee5e6b4b0d3255bfef95601890afd80709");
        }

        private static MimePart CreateMimePart(string contentType, string content)
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            return new MimePart(contentType)
            {
                ContentObject = new ContentObject(stream, ContentEncoding.EightBit)
            };
        }
    }
}
