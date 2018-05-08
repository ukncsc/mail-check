using System;
using System.Linq;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    [TestFixture]
    public class PolicyOverrideReasonDeserialiserTests
    {

        private PolicyOverrideReasonDeserialiser _policyOverrideReasonDeserialiser;

        [SetUp]
        public void SetUp()
        {
            _policyOverrideReasonDeserialiser = new PolicyOverrideReasonDeserialiser();
        }

        [Test]
        public void CorrectlyFormedPolicyOverrideReasonsGeneratesPolicyOverrideReason()
        {
            XElement xElement = XElement.Parse(PolicyOverrideReasonDeserialserTestsResources.PolicyOverrideReasonStandard);
            PolicyOverrideReason[] policyOverrideReasons = _policyOverrideReasonDeserialiser.Deserialise(new []{ xElement });

            Assert.That(policyOverrideReasons.First().PolicyOverride, Is.EqualTo(PolicyOverride.forwarded));
            Assert.That(policyOverrideReasons.First().Comment, Is.EqualTo(TestConstants.ExpectedComment));
        }

        [Test]
        public void MultipleCorrectlyFormedPolicyOverrideReasonsGeneratesMultiplePolicyOverrideReason()
        {
            XElement xElement = XElement.Parse(PolicyOverrideReasonDeserialserTestsResources.PolicyOverrideReasonStandard);
            PolicyOverrideReason[] policyOverrideReasons = _policyOverrideReasonDeserialiser.Deserialise(new[] { xElement, xElement });
            Assert.That(policyOverrideReasons.Length, Is.EqualTo(2));
        }

        [Test]
        public void AllTagsMustBePolicyOverrideReason()
        {
            XElement xElement1 = XElement.Parse(PolicyOverrideReasonDeserialserTestsResources.PolicyOverrideReasonStandard);
            XElement xElement2 = XElement.Parse(PolicyOverrideReasonDeserialserTestsResources.NotReason);
            Assert.Throws<ArgumentException>(() => _policyOverrideReasonDeserialiser.Deserialise(new[] { xElement1, xElement2 }));
        }

        [Test]
        public void TypeOptional()
        {
            XElement xElement = XElement.Parse(PolicyOverrideReasonDeserialserTestsResources.NoType);
            PolicyOverrideReason[] policyOverrideReasons = _policyOverrideReasonDeserialiser.Deserialise(new[] { xElement });

            Assert.That(policyOverrideReasons.First().PolicyOverride, Is.Null);
        }

        [Test]
        public void CommentOptional()
        {
            XElement xElement = XElement.Parse(PolicyOverrideReasonDeserialserTestsResources.NoComment);
            PolicyOverrideReason[] policyOverrideReasons = _policyOverrideReasonDeserialiser.Deserialise(new[] { xElement });

            Assert.That(policyOverrideReasons.First().Comment, Is.Null);
        }

        [Test]
        public void TypeMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(PolicyOverrideReasonDeserialserTestsResources.DuplicateType);
            Assert.Throws<InvalidOperationException>(() => _policyOverrideReasonDeserialiser.Deserialise(new[] { xElement }));
        }

        [Test]
        public void CommentMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(PolicyOverrideReasonDeserialserTestsResources.DuplicateComment);
            Assert.Throws<InvalidOperationException>(() => _policyOverrideReasonDeserialiser.Deserialise(new[] { xElement }));
        }

        [Test]
        public void TagsMustBeDirectDecendants()
        {
            XElement xElement = XElement.Parse(PolicyOverrideReasonDeserialserTestsResources.NotDirectDescendant);
            PolicyOverrideReason[] policyOverride =  _policyOverrideReasonDeserialiser.Deserialise(new[] { xElement });

            Assert.That(policyOverride.First().PolicyOverride, Is.Null);
            Assert.That(policyOverride.First().Comment, Is.Null);
        }
    }   
}
