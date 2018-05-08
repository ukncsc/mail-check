using System;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.Common.Converters
{
    [TestFixture]
    public class DeliveryResultsConverterTests
    {
        private DeliveryResultConverter _deliveryResultsConverter;

        [SetUp]
        public void SetUp()
        {
            _deliveryResultsConverter = new DeliveryResultConverter(A.Fake<ILogger>());
        }

        [Test]
        public void ValidValueCorrectlyConverted()
        {
            DeliveryResult? deliveryResult = _deliveryResultsConverter.Convert("delivered", "", false);
            Assert.That(deliveryResult, Is.EqualTo(DeliveryResult.Delivered));
        }

        [Test]
        public void InvalidValueReturnsNull()
        {
            DeliveryResult? deliveryResult = _deliveryResultsConverter.Convert("asdfsa", "", false);
            Assert.That(deliveryResult, Is.Null);
        }

        [Test]
        public void InvalidValueWithMandatoryConvertThrows()
        {
            Assert.Throws<ArgumentException>(() =>_deliveryResultsConverter.Convert("asdfsa", "", true));
        }
    }
}
