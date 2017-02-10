namespace Dmarc.AggregateReport.Parser.Common.Domain
{
    public class AggregateReportInfo
    {
        public AggregateReportInfo(Dmarc.AggregateReport aggregateReport, 
            EmailMetadata emailMetadata,
            AttachmentMetadata attachmentMetadata)
        {
            AggregateReport = aggregateReport;
            EmailMetadata = emailMetadata;
            AttachmentMetadata = attachmentMetadata;
        }

        public Dmarc.AggregateReport AggregateReport { get; }
        public EmailMetadata EmailMetadata { get;}
        public AttachmentMetadata AttachmentMetadata { get; }
    }
}
