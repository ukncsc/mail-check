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
    public class AuthFailureConverterTests
    {
        private AuthFailureConverter _authFailureConverter;

        [SetUp]
        public void SetUp()
        {
            _authFailureConverter = new AuthFailureConverter(A.Fake<ILogger>());
        }

        [Test]
        public void ValidValueCorrectlyConverted()
        {
            AuthFailure? authFailure = _authFailureConverter.Convert("dmarc", "", false);
            Assert.That(authFailure, Is.EqualTo(AuthFailure.Dmarc));
        }

        [Test]
        public void InvalidValueReturnsNull()
        {
            AuthFailure? authFailure = _authFailureConverter.Convert("asdf", "", false);
            Assert.That(authFailure, Is.Null);
        }

        [Test]
        public void InvalidValueWithMandatoryConvertThrows()
        {
            Assert.Throws<ArgumentException>(() => _authFailureConverter.Convert("asdf", "", true));
        }
    }
}
