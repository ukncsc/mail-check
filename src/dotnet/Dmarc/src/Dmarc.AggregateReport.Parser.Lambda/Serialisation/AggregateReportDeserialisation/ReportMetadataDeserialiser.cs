using System;
using System.Linq;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Utils;

namespace Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation
{
    public interface IReportMetadataDeserialiser
    {
        ReportMetadata Deserialise(XElement element);
    }

    public class ReportMetadataDeserialiser : IReportMetadataDeserialiser
    {
        public ReportMetadata Deserialise(XElement reportMetadata)
        {
            if (reportMetadata.Name != "report_metadata")
            {
                throw new ArgumentException("Root element must be report_metadata");
            }

            string orgName = reportMetadata.Single("org_name").Value;
            string email = reportMetadata.Single("email").Value;
            string extraContactInfo = reportMetadata.FirstOrDefault("extra_contact_info")?.Value;
            string reportId = reportMetadata.Single("report_id").Value;
            DateRange dateRange = CreateDataRange(reportMetadata.Single("date_range"));
            string[] errors = reportMetadata.Where("error").Select(_ => _.Value).ToArray();

            return new ReportMetadata(orgName, email, extraContactInfo, reportId, dateRange, errors);
        }

        private DateRange CreateDataRange(XElement dataRange)
        {
            int beginDate = int.Parse(dataRange.Single("begin").Value);
            int endDate = int.Parse(dataRange.Single("end").Value);

            return new DateRange(beginDate, endDate);
        }
    }
}