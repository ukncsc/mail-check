namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal class SpfAuthResult
    {
        public long Id { get; set; }
        public long RecordId { get; set; }
        public string Domain { get; set; }
        public SpfResult? Result { get; set; }
    }
}