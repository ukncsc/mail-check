namespace Dmarc.Common.Report.Domain
{
    public abstract class ReportInfo
    {
        public EmailMetadata EmailMetadata { get; }

        protected ReportInfo(EmailMetadata emailMetadata)
        {
            EmailMetadata = emailMetadata;
        }
    }
}