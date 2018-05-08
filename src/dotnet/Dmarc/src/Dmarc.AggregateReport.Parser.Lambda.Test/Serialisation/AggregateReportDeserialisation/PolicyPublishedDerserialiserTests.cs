using System;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using Dmarc.Common.Validation;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    [TestFixture]
    public class PolicyPublishedDerserialiserTests
    {
        private PolicyPublishedDeserialiser _policyPublishedDeserialiser;
        private IDomainValidator _domainValidator;

        [SetUp]
        public void SetUp()
        {
            _domainValidator = A.Fake<IDomainValidator>();
            _policyPublishedDeserialiser = new PolicyPublishedDeserialiser(_domainValidator);
        }

        [Test]
        public void PolicyPublishedMustExist()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.NoPolicyPublished);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            Assert.Throws<ArgumentException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void DomainMustExist()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.NoDomain);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            Assert.Throws<InvalidOperationException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void AdkimOptional()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.NoAdkim);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            PolicyPublished policyPublished = _policyPublishedDeserialiser.Deserialise(xElement);
            Assert.That(policyPublished.Adkim, Is.Null);
        }

        [Test]
        public void PMustExist()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.NoP);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            Assert.Throws<InvalidOperationException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void SpOptional()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.NoSp);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            PolicyPublished policyPublished = _policyPublishedDeserialiser.Deserialise(xElement);
            Assert.That(policyPublished.Sp, Is.Null);
        }

        [Test]
        public void PctOptional()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.NoPct);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            PolicyPublished policyPublished = _policyPublishedDeserialiser.Deserialise(xElement);
            Assert.That(policyPublished.Pct, Is.Null);
        }

        [Test]
        public void CorrectlyFormedPolicyPublishedGeneratesPolicyPublished()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.PolicyPublishedStandard);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            PolicyPublished policyPublished  = _policyPublishedDeserialiser.Deserialise(xElement);

            Assert.That(policyPublished.Domain, Is.EqualTo(TestConstants.ExpectedDomain));
            Assert.That(policyPublished.Adkim, Is.EqualTo(TestConstants.ExpectedAdkimAlignment));
            Assert.That(policyPublished.Aspf, Is.EqualTo(TestConstants.ExpectedAspfAlignment));
            Assert.That(policyPublished.P, Is.EqualTo(TestConstants.ExpectedDisposition));
            Assert.That(policyPublished.Sp, Is.EqualTo(TestConstants.ExpectedDisposition));
            Assert.That(policyPublished.Pct, Is.EqualTo(TestConstants.ExpectedPct));
        }

        [Test]
        public void DomainMustBeValid()
        {
            XElement xElement = XElement.Parse(PolicyPublishedDeserialiserTestsResources.PolicyPublishedStandard);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(false);
            Assert.Throws<ArgumentException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void DomainMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.MultipleDomain);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            Assert.Throws<InvalidOperationException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void AdkimMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.MutlipleAdkim);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            Assert.Throws<InvalidOperationException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void AspfMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.MultipleAspf);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            Assert.Throws<InvalidOperationException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void PMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.MultipleP);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            Assert.Throws<InvalidOperationException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void SpMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.MultipleSp);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            Assert.Throws<InvalidOperationException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void PctMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.MultiplePct);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            Assert.Throws<InvalidOperationException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void InvalidAdkimValueProducesNullValue()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.InvalidAdkim);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            PolicyPublished policyPublished = _policyPublishedDeserialiser.Deserialise(xElement);
            Assert.That(policyPublished.Adkim, Is.Null);
        }

        [Test]
        public void InvalidSpfValueProducesNullValue()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.InvalidAspf);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            PolicyPublished policyPublished = _policyPublishedDeserialiser.Deserialise(xElement);
            Assert.That(policyPublished.Aspf, Is.Null);
        }

        [Test]
        public void PValueMustBeValid()
        {
           XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.InvalidP);
           A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
           Assert.Throws<ArgumentException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void InvalidSpValueProducesNullValue()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.InvalidSp);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            PolicyPublished policyPublished = _policyPublishedDeserialiser.Deserialise(xElement);
            Assert.That(policyPublished.Sp, Is.Null);
        }

        [Test]
        public void InvalidPctValueProducesNullValue()
        {
            XElement xElement= XElement.Parse(PolicyPublishedDeserialiserTestsResources.InvalidPct);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            PolicyPublished policyPublished = _policyPublishedDeserialiser.Deserialise(xElement);
            Assert.That(policyPublished.Pct, Is.Null);
        }

        [Test]
        public void TagsMustBeDirectDecendants()
        {
            XElement xElement = XElement.Parse(PolicyPublishedDeserialiserTestsResources.NotDirectDescendants);
            A.CallTo(() => _domainValidator.IsValidDomain(A<string>._)).Returns(true);
            Assert.Throws<InvalidOperationException>(() => _policyPublishedDeserialiser.Deserialise(xElement));
        }
    }
}
