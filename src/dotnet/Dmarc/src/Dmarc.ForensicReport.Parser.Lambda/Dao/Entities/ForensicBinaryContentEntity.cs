using System.Collections.Generic;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class ForensicBinaryContentEntity
    {
        public ForensicBinaryContentEntity(byte[] content,
            List<HashEntity> hashes)
        {
            Content = content;
            Hashes = hashes;
        }

        public long Id { get; set; }
        public byte[] Content { get; }
        public List<HashEntity> Hashes { get; }
    }
}