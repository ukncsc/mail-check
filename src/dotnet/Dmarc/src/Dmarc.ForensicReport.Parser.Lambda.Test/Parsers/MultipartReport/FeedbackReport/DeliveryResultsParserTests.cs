using System;
using System.Collections.Generic;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Parsers.MultipartReport.FeedbackReport
{
    [TestFixture]
    public class DeliveryResultsParserTests
    {
        private DeliveryResultParser _deliveryResultsParser;
        private IDeliveryResultConverter _deliveryResultConverter;

        [SetUp]
        public void SetUp()
        {
            _deliveryResultConverter = A.Fake<IDeliveryResultConverter>();
            _deliveryResultsParser = new DeliveryResultParser(_deliveryResultConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "delivered" } } };

            A.CallTo(() => _deliveryResultConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(DeliveryResult.Delivered);

            DeliveryResult? deliveryResults = _deliveryResultsParser.Parse(headers, "header1", false, false, false);

            Assert.That(deliveryResults, Is.EqualTo(DeliveryResult.Delivered));
        }

        [Test]
        public void FieldDoenstExistReturnsNull()
        {
            DeliveryResult? deliveryResults = _deliveryResultsParser.Parse(new Dictionary<string, List<string>>(), "header1", false, false, false);

            Assert.That(deliveryResults, Is.Null);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _deliveryResultsParser.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void MultipleValuesThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "delivered", "delivered" } } };
            Assert.Throws<InvalidOperationException>(() => _deliveryResultsParser.Parse(headers, "header1", false, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<InvalidOperationException>(() => _deliveryResultsParser.Parse(headers, "header1", false, true, false));
        }
    }
}
