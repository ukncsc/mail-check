namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class ForensicReportUriEntity
    {
        public ForensicReportUriEntity(ForensicUriEntity forensicUri)
        {
            ForensicUri = forensicUri;
        }

        public long ReportId { get; set; }
        public ForensicUriEntity ForensicUri { get; set; }
    }
}