namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal class PolicyOverrideReasonEntity
    {
        public long Id { get; set; }
        public long RecordId { get; set; }
        public EntityPolicyOverride? PolicyOverride { get; set; }
        public string Comment { get; set; }
    }
}