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
    public class AuthFailureParserTests
    {
        private IAuthFailureConverter _authFailureConverter;
        private AuthFailureParser _authFailureParser;

        [SetUp]
        public void SetUp()
        {
            _authFailureConverter = A.Fake<IAuthFailureConverter>();
            _authFailureParser = new AuthFailureParser(_authFailureConverter);
        }

        [Test]
        public void FieldAndValueExistReturnsValue()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "dmar" } } };

            AuthFailure expected = AuthFailure.Dmarc;
            A.CallTo(() => _authFailureConverter.Convert(A<string>._, A<string>._, A<bool>._)).Returns(expected);

            AuthFailure? authFailure = _authFailureParser.Parse(headers, "header1", false, false, false);

            Assert.That(authFailure, Is.EqualTo(expected));
        }

        [Test]
        public void FieldDoenstExistReturnsNull()
        {
            AuthFailure? authFailure = _authFailureParser.Parse(new Dictionary<string, List<string>>(), "header1", false, true, true);

            Assert.That(authFailure, Is.Null);
        }

        [Test]
        public void FieldDoesntExistFieldMandatoryThrows()
        {
            Assert.Throws<ArgumentException>(() => _authFailureParser.Parse(new Dictionary<string, List<string>>(), "header1", true, false, false));
        }

        [Test]
        public void MultipleValuesThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string> { "dmarc", "dmarc"} } };
            Assert.Throws<InvalidOperationException>(() => _authFailureParser.Parse(headers, "header1", false, false, false));
        }

        [Test]
        public void NoValueValueMandatoryThrows()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> { { "header1", new List<string>() } };
            Assert.Throws<InvalidOperationException>(() => _authFailureParser.Parse(headers, "header1", false, true, false));
        }
    }
}
