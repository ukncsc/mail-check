using System;
using Dmarc.Common.Environment;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Common.Test.Environment
{
    [TestFixture]
    public class EnvironmentVariablesTests
    {
        private EnvironmentVariables _environmentVariables;
        private IEnvironment _environment;

        [SetUp]
        public void SetUp()
        {
            _environment = A.Fake<IEnvironment>();
            _environmentVariables = new EnvironmentVariables(_environment);
        }

        [Test]
        public void GetWhenVariableExistsReturnsVariable()
        {
            string variableName = "variableName";
            string value = "variable";
            A.CallTo(() => _environment.GetEnvironmentalVariable(variableName)).Returns(value);
            string variable = _environmentVariables.Get(variableName);
            Assert.That(variable, Is.EqualTo(value));
        }

        [TestCase("", TestName = "get throws when empty string returned")]
        [TestCase("   ", TestName = "get throws when whitespace returned")]
        [TestCase(null, TestName = "get throws when null returned")]
        public void GetThrowsUnderTheseCircumstances(string value)
        {
            A.CallTo(() => _environment.GetEnvironmentalVariable(A<string>._)).Returns(value);
            Assert.Throws<ArgumentException>(() => _environmentVariables.Get("variableName"));
        }

        [Test]
        public void GetAsDoubleVariableExistsReturnsVariable()
        {
            string variableName = "variableName";
            string value = "123";
            A.CallTo(() => _environment.GetEnvironmentalVariable(variableName)).Returns(value);
            double variable = _environmentVariables.GetAsDouble(variableName);
            Assert.That(variable, Is.EqualTo(123));
        }

        [Test]
        public void GetAsDoubleThrowsIfNotDouble()
        {
            A.CallTo(() => _environment.GetEnvironmentalVariable(A<string>._)).Returns("asdfas");
            Assert.Throws<ArgumentException>(() => _environmentVariables.GetAsDouble("variableName"));
        }

        [Test]
        public void GetAsLongVariableExistsReturnVariable()
        {
            string variableName = "variableName";
            string value = "123";
            A.CallTo(() => _environment.GetEnvironmentalVariable(variableName)).Returns(value);
            double variable = _environmentVariables.GetAsLong(variableName);
            Assert.That(variable, Is.EqualTo(123));
        }

        [Test]
        public void GetAsLongThrowsIfNotLong()
        {
            A.CallTo(() => _environment.GetEnvironmentalVariable(A<string>._)).Returns("asdfas");
            Assert.Throws<ArgumentException>(() => _environmentVariables.GetAsLong("variableName"));
        }
    }
}
