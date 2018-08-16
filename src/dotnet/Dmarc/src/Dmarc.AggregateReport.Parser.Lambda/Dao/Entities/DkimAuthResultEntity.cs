namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal class DkimAuthResultEntity
    {
        public long Id { get; set; }
        public long RecordId { get; set; }
        public string Domain { get; set; }
        public string Selector { get; set; }
        public EntityDkimResult? Result { get; set; }
        public string HumanResult { get; set; }
    }
}