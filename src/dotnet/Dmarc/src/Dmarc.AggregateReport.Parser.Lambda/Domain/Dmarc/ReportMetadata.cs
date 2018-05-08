namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class ReportMetadata
    {
        public ReportMetadata()
        {
        }

        public ReportMetadata(string orgName, string email, string extraContactInfo, string reportId, DateRange range, string[] error)
        {
            OrgName = orgName;
            Email = email;
            ExtraContactInfo = extraContactInfo;
            ReportId = reportId;
            Range = range;
            Error = error;
        }

        public string OrgName { get; set; }

        public string Email { get; set; }

        public string ExtraContactInfo { get; set; }

        public string ReportId { get; set; }

        public DateRange Range { get; set; }

        public string[] Error { get; set; }
    }
}