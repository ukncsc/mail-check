using Dmarc.Common.Report.Domain;

namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public class ForensicReportInfo : ReportInfo
    {
        public ForensicReportInfo(ForensicReport forensicReport, EmailMetadata emailMetadata) 
            : base(emailMetadata)
        {
            ForensicReport = forensicReport;
        }

        public ForensicReport ForensicReport { get; }
    }
}