using System.Collections.Generic;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class Rfc822HeaderSetEntity
    {
        public Rfc822HeaderSetEntity(
            ContentTypeEntity contentType, 
            int order, 
            int depth, 
            List<Rfc822HeaderEntity> headers)
        {
            ContentType = contentType;
            Order = order;
            Depth = depth;
            Headers = headers;
        }

        public long Id { get; set; }
        public long ReportId { get; set; }
        public ContentTypeEntity ContentType { get; set; }
        public int Order { get; }
        public int Depth { get; }
        public List<Rfc822HeaderEntity> Headers { get; }
    }
}
