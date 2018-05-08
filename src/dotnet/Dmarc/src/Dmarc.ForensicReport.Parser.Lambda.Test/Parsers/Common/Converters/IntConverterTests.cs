using System;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common.Converters
{
    [TestFixture]
    public class IntConverterTests
    {
        private IntConverter _intConverter;

        [SetUp]
        public void SetUp()
        {
            _intConverter = new IntConverter(A.Fake<ILogger>());
        }

        [Test]
        public void ValidValueCorrectlyConverted()
        {
            int? intValue = _intConverter.Convert("1", "", false);
            Assert.That(intValue, Is.EqualTo(1));
        }

        [Test]
        public void InvalidValueReturnsNull()
        {
            int? intValue = _intConverter.Convert("asdf", "", false);
            Assert.That(intValue, Is.Null);
        }

        [Test]
        public void InvalidValueWithMandatoryConvertThrows()
        {
            Assert.Throws<ArgumentException>(() => _intConverter.Convert("asdf", "", true));
        }
    }
}
