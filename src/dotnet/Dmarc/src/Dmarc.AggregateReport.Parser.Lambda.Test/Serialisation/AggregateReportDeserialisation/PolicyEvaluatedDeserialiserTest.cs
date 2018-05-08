using System;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    [TestFixture]
    public class PolicyEvaluatedDeserialiserTest
    {
        private PolicyEvaluatedDeserialiser _policyEvaluatedDeserialiser;
        private IPolicyOverrideReasonDeserialiser _policyOverrideReasonDeserialiser;

        [SetUp]
        public void SetUp()
        {
            _policyOverrideReasonDeserialiser = A.Fake<IPolicyOverrideReasonDeserialiser>();
            _policyEvaluatedDeserialiser = new PolicyEvaluatedDeserialiser(_policyOverrideReasonDeserialiser);
        }

        [Test]
        public void DispositionTagOptional()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.NoDispositionTag);
            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(xElement);

            Assert.That(policyEvaluated.Disposition, Is.Null);
        }

        [Test]
        public void DkimTagOptional()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.NoDkimTag);
            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(xElement);

            Assert.That(policyEvaluated.Dkim, Is.Null);
        }

        [Test]
        public void SpfTagOptional()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.NoSpfTag);
            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(xElement);

            Assert.That(policyEvaluated.Spf, Is.Null);
        }

        [Test]
        public void CorrectlyFormedPolicyEvaluatedGeneratesPolicyEvaluated()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.StandardPolicyEvaluated);
            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(xElement);

            Assert.That(policyEvaluated.Disposition, Is.EqualTo(TestConstants.ExpectedDisposition));
            Assert.That(policyEvaluated.Dkim, Is.EqualTo(TestConstants.ExpectedDkimDmarcResult));
            Assert.That(policyEvaluated.Spf, Is.EqualTo(TestConstants.ExpectedSpfDmarcResult));
        }

        [Test]
        public void DispositionValueOptional()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.NoDispositionValue);
            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(xElement);

            Assert.That(policyEvaluated.Disposition, Is.Null);
        }

        [Test]
        public void DkimValueOptional()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.NoDkimValue);
            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(xElement);

            Assert.That(policyEvaluated.Dkim, Is.Null);
        }

        [Test]
        public void SpfValueOptional()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.NoSpfValue);
            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(xElement);

            Assert.That(policyEvaluated.Spf, Is.Null);
        }

        [Test]
        public void DispositionMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.DuplicateDisposition);
            Assert.Throws<InvalidOperationException>(() => _policyEvaluatedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void DkimMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.DuplicateDkim);
            Assert.Throws<InvalidOperationException>(() => _policyEvaluatedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void SpfMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.DuplicateSpf);
            Assert.Throws<InvalidOperationException>(() => _policyEvaluatedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void InvalidDispositionProducesNullValue()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.InvalidDisposition);
            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(xElement);

            Assert.That(policyEvaluated.Disposition, Is.Null);
        }

        [Test]
        public void InvalidDkimProducesNullValue()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.InvalidDkim);
            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(xElement);

            Assert.That(policyEvaluated.Dkim, Is.Null);
        }

        [Test]
        public void SpfValueCanBeInvalid()
        {
            Assert.DoesNotThrow(() => XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.InvalidSpf));
        }

        [Test]
        public void InvalidSpfProducesNullValue()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.InvalidSpf);
            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(xElement);

            Assert.That(policyEvaluated.Spf, Is.Null);
        }

        [Test]
        public void TagMustBePolicyEvaluated()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.NotPolicyEvaluated);
            Assert.Throws<ArgumentException>(() => _policyEvaluatedDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void TagsMustBeDirectDecendants()
        {
            XElement xElement = XElement.Parse(PolicyEvaluatedDeserialiserTestsResource.NotDirectDescendant);
            Assert.Throws<InvalidOperationException>(() => _policyEvaluatedDeserialiser.Deserialise(xElement));
        }
    }
}
