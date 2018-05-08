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
    public class FeedbackTypeParserTests
    {
        private IFeedbackTypeConverter _feedbackTypeConverter;
        private FeedbackTypeParser _feedbackTypeParser;

        [SetUp]
        public void SetUp()
        {
            _feedbackTypeConverter = A.Fake<IFeedbackTypeConverter>();
            _feedbackTypeParser = new FeedbackTypeParser(_feedbackTypeConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "authfailure" } } };

            FeedbackType feedbackType = FeedbackType.AuthFailure;
            A.CallTo(() => _feedbackTypeConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(feedbackType);

            FeedbackType? feedback = _feedbackTypeParser.Parse(headers, "header1", false, false, false);

            Assert.That(feedback, Is.EqualTo(feedbackType));
        }

        [Test]
        public void FieldDoenstExistReturnsNull()
        {
            FeedbackType? feedbackType = _feedbackTypeParser.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(feedbackType, Is.Null);
        }

        [Test]
        public void FieldDoenstExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _feedbackTypeParser.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void MultipleValuesThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "authfailure", "authfailure" } } };
            Assert.Throws<InvalidOperationException>(() => _feedbackTypeParser.Parse(headers, "header1", false, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<InvalidOperationException>(() => _feedbackTypeParser.Parse(headers, "header1", false, true, false));
        }
    }
}
