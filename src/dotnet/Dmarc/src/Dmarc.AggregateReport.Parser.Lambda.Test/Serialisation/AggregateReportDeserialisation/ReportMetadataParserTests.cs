using System;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    [TestFixture]
    public class ReportMetadataParserTests
    {
        private ReportMetadataDeserialiser _reportMetadataDeserialiser;

        [SetUp]
        public void SetUp()
        {
            _reportMetadataDeserialiser = new ReportMetadataDeserialiser();
        }

        [Test]
        public void ReportMetadataTagsMustExist()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.NoReportMetadata);
            Assert.Throws<ArgumentException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void OrgNameTagMustExist()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.NoOrgName);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void EmailTagMustExist()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.NoEmail);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void ExtraContactInfoIsOptional()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.NoExtraContactInfo);
            ReportMetadata reportMetadata = _reportMetadataDeserialiser.Deserialise(xElement);
            Assert.That(reportMetadata.ExtraContactInfo, Is.Null);
        }

        [Test]
        public void ReportIdMustExist()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.NoReportId);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void DateRangeMustExist()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.NoDateRange);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void BeginMustExist()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.NoBegin);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void EndMustExist()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.NoEnd);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void ErrorsIsOptional()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.NoError);
            ReportMetadata reportMetadata = _reportMetadataDeserialiser.Deserialise(xElement);
            Assert.That(reportMetadata.Error.Length, Is.EqualTo(0));
        }


        [Test]
        public void CorrectlyFormReportMetadataGeneratesReportMetadata()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.StandardReportMetadata);
            ReportMetadata reportMetadata = _reportMetadataDeserialiser.Deserialise(xElement);

            Assert.That(reportMetadata.OrgName, Is.EqualTo(TestConstants.ExpectedOrgName));
            Assert.That(reportMetadata.Email, Is.EqualTo(TestConstants.ExpectedEmail));
            Assert.That(reportMetadata.ExtraContactInfo, Is.EqualTo(TestConstants.ExpectedExtraContactInfo));
            Assert.That(reportMetadata.ReportId, Is.EqualTo(TestConstants.ExpectedReportId));
            Assert.That(reportMetadata.Range.Begin, Is.EqualTo(TestConstants.ExpectedRangeBegin));
            Assert.That(reportMetadata.Range.End, Is.EqualTo(TestConstants.ExpectedRangeEnd));
            Assert.That(reportMetadata.Error[0], Is.EqualTo(TestConstants.ExpectedError));
        }

        [Test]
        public void OrgNameMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.MultipleOrgName);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void EmailsMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.MultipleEmail);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void MutlipleExtraContactInfosTakeFirst()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.MultipleExtraContactInfo);
            ReportMetadata reportMetadata = _reportMetadataDeserialiser.Deserialise(xElement);
            Assert.That(reportMetadata.ExtraContactInfo, Is.EqualTo(TestConstants.ExpectedExtraContactInfo));
        }

        [Test]
        public void ReportIdMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.MultipleReportId);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void DateRangeMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.MultipleDateRange);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void BeginMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.MultipleBegin);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void EndMustNotOccurMoreThanOnce()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.MultipleEnd);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void MultipleErrorsAllowed()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.MultipleError);
            ReportMetadata reportMetadata = _reportMetadataDeserialiser.Deserialise(xElement);
            Assert.That(reportMetadata.Error[0], Is.EqualTo(TestConstants.ExpectedError));
            Assert.That(reportMetadata.Error[1], Is.EqualTo(TestConstants.ExpectedError2));
        }

        [Test]
        public void BeginMustBeInt()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.BeginNotInt);
            Assert.Throws<FormatException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }

        [Test]
        public void EndMustBeInt()
        {
            XElement xElement= XElement.Parse(ReportMetadataParserTestsResources.EndNotInt);
            Assert.Throws<FormatException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }
        [Test]
        public void TagsMustBeDirectDecendants()
        {
            XElement xElement = XElement.Parse(ReportMetadataParserTestsResources.NotDirectDescendants);
            Assert.Throws<InvalidOperationException>(() => _reportMetadataDeserialiser.Deserialise(xElement));
        }
    }
}
