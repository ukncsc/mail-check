namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class ForensicUriEntity
    {
        public ForensicUriEntity(string uri, string sha256)
        {
            Uri = uri;
            Sha256 = sha256;
        }

        public long Id { get; set; }
        public string Uri { get; }
        public string Sha256 { get; }
    }
}
