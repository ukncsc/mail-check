using System;
using System.Linq;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    [TestFixture]
    public class DkimAuthResultDerserialiserTests
    {
        private DkimAuthResultDeserialiser _dkimAuthResultDeserialiser;

        [SetUp]
        public void SetUp()
        {
            _dkimAuthResultDeserialiser = new DkimAuthResultDeserialiser();
        }

        [Test]
        public void DomainTagMustExist()
        {
            XElement xElement = XElement.Parse(DkimAuthResultSerialiserTestsResources.NoDomainTag);
            Assert.Throws<InvalidOperationException>(() => _dkimAuthResultDeserialiser.Deserialise(new [] {xElement}));
        }

        [Test]
        public void ResultTagMustExist()
        {
            XElement xElement = XElement.Parse(DkimAuthResultSerialiserTestsResources.NoResultTag);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults.First().Result, Is.Null);
        }

        [Test]
        public void HumanResultTagIsOptional()
        {
            XElement xElement = XElement.Parse(DkimAuthResultSerialiserTestsResources.NoHumanResultTag);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults.First().HumanResult, Is.Null);
        }

        [Test]
        public void SingleCorrectlyFormedDkimAuthResultGeneratesDkimAuthResult()
        {
            XElement xElement = XElement.Parse(DkimAuthResultSerialiserTestsResources.StandardDkimAuthResult);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});

            Assert.That(dkimAuthResults.First().Domain, Is.EqualTo(TestConstants.ExpectedDomain));
            Assert.That(dkimAuthResults.First().Result, Is.EqualTo(TestConstants.ExpectedDkimResult));
            Assert.That(dkimAuthResults.First().HumanResult, Is.EqualTo(TestConstants.ExpectedHumanResult));
        }

        [Test]
        public void MultipleCorrectlyFormedDkimAuthResultGeneratesMultipleDkimAuthResults()
        {
            XElement xElement = XElement.Parse(DkimAuthResultSerialiserTestsResources.StandardDkimAuthResult);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new[] { xElement, xElement });

            Assert.That(dkimAuthResults.Length, Is.EqualTo(2));
        }

        [Test]
        public void DomainValueOptional()
        {
            XElement xElement = XElement.Parse(DkimAuthResultSerialiserTestsResources.NoDomainValue);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults.First().Domain, Is.Empty);
        }

        [Test]
        public void ResultValueOptional()
        {
            XElement xElement = XElement.Parse(DkimAuthResultSerialiserTestsResources.NoResultValue);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults.First().Result, Is.Null);
        }

        [Test]
        public void HumanResultValueOptional()
        {
            XElement xElement = XElement.Parse(DkimAuthResultSerialiserTestsResources.NoHumanResultValue);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults.First().HumanResult, Is.Empty);
        }

        [Test]
        public void InvalidResultValueNull()
        {
            XElement xElement = XElement.Parse(DkimAuthResultSerialiserTestsResources.InvalidResultValue);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults.First().Result, Is.Null);
        }

        [Test]
        public void MustAllBeDkimAuthResults()
        {
            XElement xElement1 = XElement.Parse(DkimAuthResultSerialiserTestsResources.StandardDkimAuthResult);
            XElement xElement2 = XElement.Parse(DkimAuthResultSerialiserTestsResources.NotDkim);
            Assert.Throws<ArgumentException>(() => _dkimAuthResultDeserialiser.Deserialise(new[] { xElement1, xElement2 }));
        }

        [Test]
        public void TagsMustBeDirectDecendants()
        {
            XElement xElement = XElement.Parse(DkimAuthResultSerialiserTestsResources.NotDirectDescendants);
            Assert.Throws<InvalidOperationException>(() => _dkimAuthResultDeserialiser.Deserialise(new[] { xElement }));
        }
    }
}
