namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal class DkimAuthResult
    {
        public long Id { get; set; }
        public long RecordId { get; set; }
        public string Domain { get; set; }
        public DkimResult? Result { get; set; }
        public string HumanResult { get; set; }
    }
}