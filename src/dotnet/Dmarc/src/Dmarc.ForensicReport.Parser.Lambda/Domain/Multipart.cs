namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public class Multipart : EmailPart
    {
        public Multipart(string contentType, int depth, Disposition disposition) 
            : base(contentType, depth, disposition)
        {
        }
    }
}