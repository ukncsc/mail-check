namespace Dmarc.AggregateReport.Parser.Common.Domain
{
    public class EmailMetadata
    {
        public EmailMetadata(string orginalUri, string filename, long fileSizeKb)
            : this(string.Empty, orginalUri, filename, fileSizeKb)
        {
        }

        public EmailMetadata(string requestId, string originalUri, string filename, long fileSizeKb)
        {
            RequestId = requestId;
            OriginalUri = originalUri;
            Filename = filename;
            FileSizeKb = fileSizeKb;
        }

        public string RequestId { get; }
        public string OriginalUri { get; }
        public string Filename { get; }
        public long FileSizeKb { get; }

    }
}