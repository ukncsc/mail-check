using System.IO;

namespace Dmarc.AggregateReport.Parser.Common.Domain
{
    public class EmailMessageInfo
    {
        public EmailMessageInfo(
            EmailMetadata emailMetadata, 
            Stream emailStream)
        {
            EmailMetadata = emailMetadata;
            EmailStream = emailStream;
        }

        public EmailMetadata EmailMetadata { get; }
        public Stream EmailStream { get; }
    }
}
