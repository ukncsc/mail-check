namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class Rfc822HeaderTextValueEntity
    {
        public Rfc822HeaderTextValueEntity(string value)
        {
            Value = value;
        }

        public long Id { get; set; }

        public string Value { get; }
    }
}