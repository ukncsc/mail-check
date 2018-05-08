using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Hashing;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body.EmailParts
{
    public interface IMimePartParser
    {
        MimeContent Parse(MimePart mimeEntity, int depth);
    }

    public class MimePartParser : IMimePartParser
    {
        private readonly IEnumerable<IHashInfoCalculator> _hashInfoCalculators;

        public MimePartParser(IEnumerable<IHashInfoCalculator> hashInfoCalculators)
        {
            _hashInfoCalculators = hashInfoCalculators;
        }

        public MimeContent Parse(MimePart mimePart, int depth)
        {
            using (Stream stream = mimePart.ContentObject.Open())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);

                    List<HashInfo> hashInfos = _hashInfoCalculators.Select(_ => _.Calculate(mimePart)).ToList();

                    Disposition disposition = mimePart.ContentDisposition == null
                        ? null
                        : new Disposition(mimePart.ContentDisposition.IsAttachment, mimePart.ContentDisposition.FileName);

                    return new MimeContent(mimePart.ContentType.MimeType, depth, disposition, memoryStream.ToArray(), hashInfos);
                }
            }
        }
    }
}
