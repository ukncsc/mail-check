using System.Net.Mail;

namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public class ReceivedHeader
    {
        public ReceivedHeader(string from, string by, MailAddress @for)
        {
            From = from;
            By = by;
            For = @for;
        }

        public string From { get; }
        public string By { get; }
        public MailAddress For { get; }
    }
}