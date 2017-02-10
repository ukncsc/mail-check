namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal class PolicyOverrideReason
    {
        public long Id { get; set; }
        public long RecordId { get; set; }
        public PolicyOverride? PolicyOverride { get; set; }
        public string Comment { get; set; }
    }
}