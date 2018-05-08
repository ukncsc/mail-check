using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class ForensicTextEntity
    {
        public ForensicTextEntity(ForensicTextContentEntity forensicTextContent,
            ContentTypeEntity contentType,
            int order,
            int depth)
        {
            ForensicTextContent = forensicTextContent;
            ContentType = contentType;
            Order = order;
            Depth = depth;
        }

        public long ReportId { get; set; }
        public ForensicTextContentEntity ForensicTextContent { get; set; }
        public ContentTypeEntity ContentType { get; set; }
        public int Order { get; }
        public int Depth { get; }
    }
}
