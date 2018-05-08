namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class ContentTypeEntity
    {
        public ContentTypeEntity(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; }
    }
}
