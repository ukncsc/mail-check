namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal class SpfAuthResultEntity
    {
        public long Id { get; set; }
        public long RecordId { get; set; }
        public string Domain { get; set; }
        public EntitySpfDomainScope? Scope { get; set; }
        public EntitySpfResult? Result { get; set; }
    }
}