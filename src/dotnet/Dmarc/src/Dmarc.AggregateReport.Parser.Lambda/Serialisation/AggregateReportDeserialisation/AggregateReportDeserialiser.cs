using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Utils;
using Dmarc.Common.Report.Domain;

namespace Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation
{
    public interface IAggregateReportDeserialiser
    {
        AggregateReportInfo Deserialise(AttachmentInfo attachment, EmailMetadata emailMetadata);
    }

    public class AggregateReportDeserialiser : IAggregateReportDeserialiser
    {
        private readonly IReportMetadataDeserialiser _reportMetadataDeserialiser;
        private readonly IPolicyPublishedDeserialiser _policyPublishedDeserialiser;
        private readonly IRecordDeserialiser _recordDeserialiser;

        public AggregateReportDeserialiser(IReportMetadataDeserialiser reportMetadataDeserialiser,
            IPolicyPublishedDeserialiser policyPublishedDeserialiser,
            IRecordDeserialiser recordDeserialisery)
        {
            _reportMetadataDeserialiser = reportMetadataDeserialiser;
            _policyPublishedDeserialiser = policyPublishedDeserialiser;
            _recordDeserialiser = recordDeserialisery;
        }

        public AggregateReportInfo Deserialise(AttachmentInfo attachment, EmailMetadata emailMetadata)
        {
            using (Stream stream = attachment.GetStream())
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    using (XmlReader reader = XmlReader.Create(streamReader))
                    {
                        XDocument document = XDocument.Load(reader);

                        XElement feedback = document.Root;
                        if (document.Root.Name != "feedback")
                        {
                            throw new ArgumentException("Root of aggregate report must be feedback.");
                        }

                        XElement versionElement = feedback.SingleOrDefault("version");
                        double candidateVersion;
                        double? version = double.TryParse(versionElement?.Value, out candidateVersion) 
                            ? candidateVersion 
                            : (double?)null;

                        ReportMetadata reportMetadata = _reportMetadataDeserialiser.Deserialise(feedback.Single("report_metadata"));
                        PolicyPublished policyPublished = _policyPublishedDeserialiser.Deserialise(feedback.Single("policy_published"));

                        IEnumerable<XElement> recordElements = feedback.Where("record").ToList();

                        if (!recordElements.Any())
                        {
                            throw new ArgumentException("Aggregate report must contain at least 1 record.");
                        }

                        Record[] records = _recordDeserialiser.Deserialise(recordElements);

                        Domain.Dmarc.AggregateReport aggregateReport = new Domain.Dmarc.AggregateReport(version, reportMetadata, policyPublished, records);
                        return new AggregateReportInfo(aggregateReport, emailMetadata, attachment.AttachmentMetadata);
                    }
                }
            }
        }
    }
}