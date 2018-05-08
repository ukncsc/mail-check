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
    public class FeedbackTypeConverterTests
    {
        private FeedbackTypeConverter _feedbackTypeConverter;

        [SetUp]
        public void SetUp()
        {
            _feedbackTypeConverter = new FeedbackTypeConverter(A.Fake<ILogger>());
        }

        [Test]
        public void ValidValueCorrectlyConverted()
        {
            FeedbackType? feedbackType = _feedbackTypeConverter.Convert("authfailure", "", false);
            Assert.That(feedbackType, Is.EqualTo(FeedbackType.AuthFailure));
        }

        [Test]
        public void InvalidValueReturnsNull()
        {
            FeedbackType? feedbackType = _feedbackTypeConverter.Convert("asdfsa", "", false);
            Assert.That(feedbackType, Is.Null);
        }

        [Test]
        public void InvalidValueWithMandatoryConvertThrows()
        {
            Assert.Throws<ArgumentException>(() => _feedbackTypeConverter.Convert("asdfsa", "", true));
        }
    }
}
