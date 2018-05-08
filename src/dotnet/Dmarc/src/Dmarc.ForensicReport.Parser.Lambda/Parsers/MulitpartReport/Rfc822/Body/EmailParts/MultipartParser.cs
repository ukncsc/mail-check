using System;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using MimeKit;
using Multipart = Dmarc.ForensicReport.Parser.Lambda.Domain.Multipart;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body.EmailParts
{
    public interface IMultipartParser
    {
        Multipart Parse(MimeEntity mimeEntity, int depth);
    }

    public class MultipartParser : IMultipartParser
    {
        public Multipart Parse(MimeEntity mimeEntity, int depth)
        {
            if (!mimeEntity.ContentType.IsMimeType(MimeTypes.Multipart, MimeTypes.Wildcard))
            {
                throw new ArgumentException($"Expected ContentType MediaType to be {MimeTypes.Multipart} but was {mimeEntity.ContentType.MediaType}.");
            }

            Disposition disposition = mimeEntity.ContentDisposition == null
                ? null
                : new Disposition(mimeEntity.ContentDisposition.IsAttachment, mimeEntity.ContentDisposition.FileName);

            return new Multipart(mimeEntity.ContentType.MimeType, depth, disposition);
        }
    }
}
