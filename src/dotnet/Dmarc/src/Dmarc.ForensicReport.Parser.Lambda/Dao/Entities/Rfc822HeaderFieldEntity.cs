namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class Rfc822HeaderFieldEntity
    {
        public Rfc822HeaderFieldEntity(string name, EntityRfc822HeaderValueType valueType)
        {
            Name = name;
            ValueType = valueType;
        }

        public long Id { get; set; }
        public string Name { get; }
        public EntityRfc822HeaderValueType ValueType { get; }
    }
}