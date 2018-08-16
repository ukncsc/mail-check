namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class AggregateReport
    {
        public AggregateReport()
        {
        }

        public AggregateReport(double? version, ReportMetadata reportMetadata, PolicyPublished policyPublished, Record[] records)
        {
            Version = version;
            ReportMetadata = reportMetadata;
            PolicyPublished = policyPublished;
            Records = records;
        }

        public double? Version { get; set; }

        public ReportMetadata ReportMetadata { get; set; }

        public PolicyPublished PolicyPublished { get; set; }

        public Record[] Records { get; set; }
    }
}