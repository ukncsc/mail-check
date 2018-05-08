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
    public class Md5HashInfoCalculatorTests
    {
        private Md5HashInfoCalculator _md5HashInfoCalculator;

        [SetUp]
        public void SetUp()
        {
            _md5HashInfoCalculator = new Md5HashInfoCalculator();
        }

        [TestCaseSource(nameof(CreateTestData))]
        public void Test(MimePart mimePart, string expectedHashValue)
        {
            HashInfo hashInfo = _md5HashInfoCalculator.Calculate(mimePart);
            Assert.That(hashInfo.Hash, Is.EqualTo(expectedHashValue));
        }

        public static IEnumerable<TestCaseData> CreateTestData()
        {
            yield return new TestCaseData(CreateMimePart("text/html", "<!DOCTYPE html><html><head><title>Title</title></head><body></body></html>"), "4242695794a5fb8d961eb2b5325cc2b5");
            yield return new TestCaseData(CreateMimePart("text/html", ""), "d41d8cd98f00b204e9800998ecf8427e");
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
