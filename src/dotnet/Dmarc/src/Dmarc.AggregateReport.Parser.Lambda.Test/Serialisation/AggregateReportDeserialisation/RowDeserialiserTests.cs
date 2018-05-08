using System;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    [TestFixture]
    public class RowDeserialiserTests
    {
        private RowDeserialiser _rowDeserialiser;
        private IPolicyEvaluatedDeserialiser _policyEvaluatedDerserialiser;

        [SetUp]
        public void SetUp()
        {
            _policyEvaluatedDerserialiser = A.Fake<IPolicyEvaluatedDeserialiser>();
            _rowDeserialiser = new RowDeserialiser(_policyEvaluatedDerserialiser);
        }

        [Test]
        public void CorrectlyFormedRowGeneratesRow()
        {
            XElement xElement = XElement.Parse(RowDeserialiserTestsResource.StandardRow);
            Row row = _rowDeserialiser.Deserialise(xElement);

            Assert.That(row, Is.Not.Null);
            Assert.That(row.SourceIp, Is.EqualTo(TestConstants.ExpectedIpAddress));
            Assert.That(row.Count, Is.EqualTo(TestConstants.ExpectedCount));
        }

        [Test]
        public void RootMustBeRowTag()
        {
            XElement xElement = XElement.Parse(RowDeserialiserTestsResource.NotRow);
            Assert.Throws<ArgumentException>(() => _rowDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void SourceIpMustExist()
        {
            XElement xElement = XElement.Parse(RowDeserialiserTestsResource.NoIpTag);
            Assert.Throws<InvalidOperationException>(() => _rowDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void CountMustExist()
        {
            XElement xElement = XElement.Parse(RowDeserialiserTestsResource.NoCount);
            Assert.Throws<InvalidOperationException>(() => _rowDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void PolicyEvaluatedMustExist()
        {
            XElement xElement = XElement.Parse(RowDeserialiserTestsResource.NoPolicyEvaluated);
            Assert.Throws<InvalidOperationException>(() => _rowDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void SourceIpMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(RowDeserialiserTestsResource.DuplicateIpTags);
            Assert.Throws<InvalidOperationException>(() => _rowDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void CountMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(RowDeserialiserTestsResource.DuplicateCountTags);
            Assert.Throws<InvalidOperationException>(() => _rowDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void PolicyEvaluatedMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(RowDeserialiserTestsResource.DuplicatePolicyEvaluatedTags);
            Assert.Throws<InvalidOperationException>(() => _rowDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void CountMustBeValidInt()
        {
            XElement xElement = XElement.Parse(RowDeserialiserTestsResource.InvalidCount);
            Assert.Throws<FormatException>(() => _rowDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void TagsMustBeDirectDecendants()
        {
            XElement xElement = XElement.Parse(RowDeserialiserTestsResource.NotDirectDescendants);
            Assert.Throws<InvalidOperationException>(() => _rowDeserialiser.Deserialise(xElement));
        }
    }
}
