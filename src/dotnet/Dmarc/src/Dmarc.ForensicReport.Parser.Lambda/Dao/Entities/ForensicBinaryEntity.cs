namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class ForensicBinaryEntity
    {
        public ForensicBinaryEntity(ForensicBinaryContentEntity forensicBinaryContent, 
            ContentTypeEntity contentType,
            string filename,
            string extension,
            ContentDisposition? disposition,
            int order,
            int depth)
        {
            ForensicBinaryContent = forensicBinaryContent;
            ContentType = contentType;
            Filename = filename;
            Extension = extension;
            Disposition = disposition;
            Order = order;
            Depth = depth;
        }

        public long ReportId { get; set; }
        public ForensicBinaryContentEntity ForensicBinaryContent { get; set; }
        public ContentTypeEntity ContentType { get; set; }
        public string Filename { get; }
        public string Extension { get; }
        public ContentDisposition? Disposition { get; }
        public int Order { get; }
        public int Depth { get; }
    }
}