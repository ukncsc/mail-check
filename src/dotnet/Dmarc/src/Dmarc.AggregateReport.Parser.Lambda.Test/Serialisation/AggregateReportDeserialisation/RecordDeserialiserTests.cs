using System;
using System.Linq;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    [TestFixture]
    public class RecordDeserialiserTests
    {
        private RecordDeserialiser _recordDeserialiser;
        private IRowDeserialiser _rowDeserialiser;
        private IIdentifiersDeserialiser _identifiersDeserialiser;
        private IAuthResultDeserialiser _authResultDeserialiser;

        [SetUp]
        public void SetUp()
        {
            _rowDeserialiser = A.Fake<IRowDeserialiser>();
            _identifiersDeserialiser = A.Fake<IIdentifiersDeserialiser>();
            _authResultDeserialiser = A.Fake<IAuthResultDeserialiser>();
            _recordDeserialiser = new RecordDeserialiser(_rowDeserialiser, _identifiersDeserialiser, _authResultDeserialiser);
        }

        [Test]
        public void RecordTagsMustExist()
        {
            XElement xElement = XElement.Parse(RecordDeserialiserTestsResource.NoRecordTag);
            Assert.Throws<ArgumentException>(() => _recordDeserialiser.Deserialise(new[] { xElement }));
        }

        [Test]
        public void MustAllBeRecordTags()
        {
            XElement xElement1 = XElement.Parse(RecordDeserialiserTestsResource.NoRecordTag);
            XElement xElement2 = XElement.Parse(RecordDeserialiserTestsResource.StandardRecord);
            Assert.Throws<ArgumentException>(() => _recordDeserialiser.Deserialise(new[] { xElement1, xElement2 }));
        }

        [Test]
        public void CorrectlyFormedRecordGenereratesRecord()
        {
            XElement xElement = XElement.Parse(RecordDeserialiserTestsResource.StandardRecord);
            Record[] records = _recordDeserialiser.Deserialise(new[] {xElement});
            Assert.That(records.Length, Is.EqualTo(1));
            Assert.That(records.First(), Is.Not.Null);
        }

        [Test] public void MultipleCorrectlyFormedRecordGenereratesMultipleRecords()
        {
            XElement xElement = XElement.Parse(RecordDeserialiserTestsResource.StandardRecord);
            Record[] records = _recordDeserialiser.Deserialise(new[] { xElement, xElement });
            Assert.That(records.Length, Is.EqualTo(2));
            Assert.That(records[0], Is.Not.Null);
            Assert.That(records[0], Is.Not.Null);
        }

        [Test]
        public void RowTagMustExist()
        {
            XElement xElement = XElement.Parse(RecordDeserialiserTestsResource.NoRow);
            Assert.Throws<InvalidOperationException>(() => _recordDeserialiser.Deserialise(new[] { xElement }));
        }

        [Test]
        public void IdentifiersTagMustExist()
        {
            XElement xElement = XElement.Parse(RecordDeserialiserTestsResource.NoIdentifiers);
            Assert.Throws<InvalidOperationException>(() => _recordDeserialiser.Deserialise(new[] { xElement }));
        }

        [Test]
        public void AuthTagOptional()
        {
            XElement xElement = XElement.Parse(RecordDeserialiserTestsResource.NoAuthResults);
            Record[] records =  _recordDeserialiser.Deserialise(new[] { xElement });
            Assert.That(records.Length, Is.EqualTo(1));
            Assert.That(records.First(), Is.Not.Null);
        }

        [Test]
        public void RowTagMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(RecordDeserialiserTestsResource.DuplicateRow);
            Assert.Throws<InvalidOperationException>(() => _recordDeserialiser.Deserialise(new[] { xElement }));
        }

        [Test]
        public void IdentifierTagMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(RecordDeserialiserTestsResource.DuplicateIdentifiers);
            Assert.Throws<InvalidOperationException>(() => _recordDeserialiser.Deserialise(new[] { xElement }));
        }

        [Test]
        public void AuthTagsMustNotOccurMoreThanOnce()
        {
            XElement xElement = XElement.Parse(RecordDeserialiserTestsResource.DuplicateAuthResults);
            Assert.Throws<InvalidOperationException>(() => _recordDeserialiser.Deserialise(new[] { xElement }));
        }

        [Test]
        public void TagsMustBeDirectDecendants()
        {
            XElement xElement = XElement.Parse(RecordDeserialiserTestsResource.NotDirectDescendants);
            Assert.Throws<InvalidOperationException>(() => _recordDeserialiser.Deserialise(new[] { xElement }));
        }
    }
}
