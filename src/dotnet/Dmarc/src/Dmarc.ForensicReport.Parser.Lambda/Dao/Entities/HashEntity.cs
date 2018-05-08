using Dmarc.ForensicReport.Parser.Lambda.Dao.Utils;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class HashEntity
    {
        public HashEntity(EntityHashType type, string hash)
        {
            Type = type;
            Hash = hash;
        }

        public long ContentId { get; set; }
        public EntityHashType Type { get; }
        public string Hash { get; }
    }
}