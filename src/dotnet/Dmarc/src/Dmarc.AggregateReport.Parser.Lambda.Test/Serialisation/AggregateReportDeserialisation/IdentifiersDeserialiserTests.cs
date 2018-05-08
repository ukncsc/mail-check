using System;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    [TestFixture]
    public class IdentifiersDeserialiserTests
    {
        private IdentifiersDeserialiser _identifiersDeserialiser;

        [SetUp]
        public void SetUp()
        {
            _identifiersDeserialiser = new IdentifiersDeserialiser();
        }
       
        [Test]
        public void HeaderFromTagMustExist()
        {
            XElement xElement = XElement.Parse(IdentifiersDeserialiserTestsResources.NoHeaderFromTag);
            Assert.Throws<InvalidOperationException>(() => _identifiersDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void EnvelopeToTagOptional()
        {
            XElement xElement = XElement.Parse(IdentifiersDeserialiserTestsResources.NoEnvelopeToTag);
            Identifier identifier = _identifiersDeserialiser.Deserialise(xElement);
            Assert.That(identifier.EnvelopeTo,Is.Null);
        }

        [Test]
        public void CorrectlyFormedIdentifiersGeneratesIndentifers()
        {
            XElement xElement = XElement.Parse(IdentifiersDeserialiserTestsResources.StandardIdentifiers);
            Identifier identifier = _identifiersDeserialiser.Deserialise(xElement);

            Assert.That(identifier.HeaderFrom, Is.EqualTo(TestConstants.ExpectedHeaderFrom));
            Assert.That(identifier.EnvelopeTo, Is.EqualTo(TestConstants.ExpectedEnvelopeTo));
        }

        [Test]
        public void EmptyHeaderFromValueProducesEmptyValue()
        {
            XElement xElement = XElement.Parse(IdentifiersDeserialiserTestsResources.NoHeaderFromValue);
            Identifier identifier = _identifiersDeserialiser.Deserialise(xElement);
            Assert.That(identifier.HeaderFrom, Is.Empty);
        }

        [Test]
        public void EmptyEnvelopeToValueProducesNullValue()
        {
            XElement xElement = XElement.Parse(IdentifiersDeserialiserTestsResources.NoEnvelopeToValue);
            Identifier identifier = _identifiersDeserialiser.Deserialise(xElement);
            Assert.That(identifier.EnvelopeTo, Is.Empty);
        }

       [Test]
        public void EnvelopeToTagMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(IdentifiersDeserialiserTestsResources.DuplicateEnvelopeToTag);
            Assert.Throws<InvalidOperationException>(() => _identifiersDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void HeaderFromTagMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(IdentifiersDeserialiserTestsResources.DuplicateHeaderFromTag);
            Assert.Throws<InvalidOperationException>(() => _identifiersDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void TagMustBeIndentifiers()
        {
            XElement xElement = XElement.Parse(IdentifiersDeserialiserTestsResources.NotIdentifiers);
            Assert.Throws<ArgumentException>(() => _identifiersDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void TagsMustBeDirectDecendants()
        {
            XElement xElement = XElement.Parse(IdentifiersDeserialiserTestsResources.NotDirectDescendant);
            Assert.Throws<InvalidOperationException>(() => _identifiersDeserialiser.Deserialise(xElement));
        }
    }
}
