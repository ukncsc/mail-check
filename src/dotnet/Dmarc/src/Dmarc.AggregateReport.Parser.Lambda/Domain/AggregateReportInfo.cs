
using Dmarc.Common.Report.Domain;

namespace Dmarc.AggregateReport.Parser.Lambda.Domain
{
    public class AggregateReportInfo : ReportInfo
    {
        public AggregateReportInfo(Dmarc.AggregateReport aggregateReport, 
            EmailMetadata emailMetadata,
            AttachmentMetadata attachmentMetadata)
            : base(emailMetadata)
        {
            AggregateReport = aggregateReport;
            AttachmentMetadata = attachmentMetadata;
        }

        public Dmarc.AggregateReport AggregateReport { get; }
        public AttachmentMetadata AttachmentMetadata { get; }
    }
}
