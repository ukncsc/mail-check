using System.Collections.Generic;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class ForensicTextContentEntity
    {
        public ForensicTextContentEntity(string text,
            List<HashEntity> hashes,
            List<ForensicTextContentUriEntity> uris)
        {
            Text = text;
            Hashes = hashes;
            Uris = uris;
        }

        public long Id { get; set; }
        public string Text { get; }
        public List<HashEntity> Hashes { get; }
        public List<ForensicTextContentUriEntity> Uris { get; }
    }
}