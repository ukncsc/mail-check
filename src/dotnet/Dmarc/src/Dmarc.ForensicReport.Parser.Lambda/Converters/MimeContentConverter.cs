using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Utils;
using Dmarc.ForensicReport.Parser.Lambda.Domain;

namespace Dmarc.ForensicReport.Parser.Lambda.Converters
{
    public interface IMimeContentConverter
    {
        ForensicBinaryEntity Convert(MimeContent mimeContent, int order);
    }

    public class MimeContentConverter : IMimeContentConverter
    {
        public ForensicBinaryEntity Convert(MimeContent mimeContent, int order)
        {
            List<HashEntity> hashes = mimeContent.Hashes.Select(_ => new HashEntity((EntityHashType)(int)_.HashType, _.Hash)).ToList();
            ForensicBinaryContentEntity forensicBinaryContentEntity = new ForensicBinaryContentEntity(mimeContent.RawContent, hashes);
            ContentTypeEntity contentType = new ContentTypeEntity(mimeContent.ContentType);
            
            return new ForensicBinaryEntity(
                forensicBinaryContentEntity,
                contentType, 
                mimeContent.Disposition?.Filename,
                mimeContent.Disposition == null ? null : Path.GetExtension(mimeContent.Disposition.Filename), 
                mimeContent.Disposition == null ? (ContentDisposition?)null : (mimeContent.Disposition.IsAttachment ? ContentDisposition.Attachment : ContentDisposition.Inline),
                order,
                mimeContent.Depth);
        }
    }
}
