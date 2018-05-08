using System;
using System.Linq;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    [TestFixture]
    public class SpfAuthResultsDeserialiserTests
    {
        private SpfAuthResultDeserialiser _spfAuthResultsDeserialiser;

        [SetUp]
        public void SetUp()
        {
            _spfAuthResultsDeserialiser = new SpfAuthResultDeserialiser();
        }

        [Test] public void CorrectlyFormedSpfAuthResultGeneratesSpfAuthResult()
        {
            XElement xElement = XElement.Parse(SpfAuthResultsDeserialiserTestsResource.SpfAuthResultStandard);
            SpfAuthResult[] spfAuthResults = _spfAuthResultsDeserialiser.Deserialise(new []{xElement});

            Assert.That(spfAuthResults.First().Domain, Is.EqualTo(TestConstants.ExpectedDomain));
            Assert.That(spfAuthResults.First().Result, Is.EqualTo(TestConstants.ExpectedSpfResult));
        }

        [Test]
        public void CorrectlyFormedMultipleSpfAuthResultGeneratesMutlipleSpfAuthResult()
        {
            XElement xElement = XElement.Parse(SpfAuthResultsDeserialiserTestsResource.SpfAuthResultStandard);
            SpfAuthResult[] spfAuthResults = _spfAuthResultsDeserialiser.Deserialise(new[] { xElement, xElement });

            Assert.That(spfAuthResults.Length, Is.EqualTo(2));
        }

        [Test]
        public void MustAllBeSpfAuthResults()
        {
            XElement xElement1 = XElement.Parse(SpfAuthResultsDeserialiserTestsResource.SpfAuthResultStandard);
            XElement xElement2 = XElement.Parse(SpfAuthResultsDeserialiserTestsResource.NotSpf);
            Assert.Throws<ArgumentException>(() => _spfAuthResultsDeserialiser.Deserialise(new[] { xElement1, xElement2 }));

        }

        [Test]
        public void DomainTagMustExist()
        {
            XElement xElement = XElement.Parse(SpfAuthResultsDeserialiserTestsResource.NoDomain);
            Assert.Throws<InvalidOperationException>(() => _spfAuthResultsDeserialiser.Deserialise(new[] { xElement }));
        }

        [Test]
        public void ResultTagMustExist()
        {
            XElement xElement = XElement.Parse(SpfAuthResultsDeserialiserTestsResource.NoResult);
            Assert.Throws<InvalidOperationException>(() => _spfAuthResultsDeserialiser.Deserialise(new[] {xElement}));
        }

        [Test]
        public void DomainTagMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(SpfAuthResultsDeserialiserTestsResource.DuplicateDomain);
            Assert.Throws<InvalidOperationException>(() => _spfAuthResultsDeserialiser.Deserialise(new[] { xElement }));
        }

        [Test]
        public void ResultTagMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(SpfAuthResultsDeserialiserTestsResource.DuplicateResult);
            Assert.Throws<InvalidOperationException>(() => _spfAuthResultsDeserialiser.Deserialise(new[] { xElement }));
        }

        [Test]
        public void InvalidResultProducesNullValue()
        {
            XElement xElement = XElement.Parse(SpfAuthResultsDeserialiserTestsResource.InvalidResult);
            SpfAuthResult[] spfAuthResults = _spfAuthResultsDeserialiser.Deserialise(new[] { xElement });
            Assert.That(spfAuthResults.First().Result, Is.Null);
        }

        [Test]
        public void TagsMustBeDirectDecendants()
        {
            XElement xElement = XElement.Parse(SpfAuthResultsDeserialiserTestsResource.NotDirectDescendants);
            Assert.Throws<InvalidOperationException>(() => _spfAuthResultsDeserialiser.Deserialise(new[] { xElement }));
        }

    }
}
