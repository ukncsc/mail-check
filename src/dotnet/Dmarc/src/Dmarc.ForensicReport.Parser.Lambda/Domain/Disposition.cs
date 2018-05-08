namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public class Disposition
    {
        public Disposition(bool isAttachment, string filename)
        {
            IsAttachment = isAttachment;
            Filename = filename;
        }

        public bool IsAttachment { get; }
        public string Filename { get; }
    }
}