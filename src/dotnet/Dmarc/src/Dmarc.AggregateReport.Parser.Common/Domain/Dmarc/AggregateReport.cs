namespace Dmarc.AggregateReport.Parser.Common.Domain.Dmarc
{
    public class AggregateReport
    {
        public AggregateReport()
        {
        }

        public AggregateReport(ReportMetadata reportMetadata, PolicyPublished policyPublished, Record[] records)
        {
            ReportMetadata = reportMetadata;
            PolicyPublished = policyPublished;
            Records = records;
        }

        public ReportMetadata ReportMetadata { get; set; }

        public PolicyPublished PolicyPublished { get; set; }

        public Record[] Records { get; set; }
    }
}