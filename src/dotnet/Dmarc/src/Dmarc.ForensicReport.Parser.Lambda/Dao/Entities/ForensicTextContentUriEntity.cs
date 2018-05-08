namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class ForensicTextContentUriEntity
    {
        public ForensicTextContentUriEntity(ForensicUriEntity forensicUri)
        {
            ForensicUri = forensicUri;
        }

        public long ForensicTextContentId { get; set; }
        public ForensicUriEntity ForensicUri { get; set; }
    }
}
