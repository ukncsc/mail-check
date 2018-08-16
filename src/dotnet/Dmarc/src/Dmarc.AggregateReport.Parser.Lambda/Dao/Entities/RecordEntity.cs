using System.Collections.Generic;

namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal class RecordEntity
    {
        public long Id { get; set; }
        public long AggregateReportId { get; set; }
        public string SourceIp { get; set; }
        public int Count { get; set; }
        public EntityDisposition? Disposition { get; set; }
        public EntityDmarcResult? Dkim { get; set; }
        public EntityDmarcResult? Spf { get; set; }
        public List<PolicyOverrideReasonEntity> Reason { get; set; } = new List<PolicyOverrideReasonEntity>();
        public string EnvelopeTo { get; set; }
        public string EnvelopeFrom { get; set; }
        public string HeaderFrom { get; set; }
        public List<DkimAuthResultEntity> DkimAuthResults { get; set; } = new List<DkimAuthResultEntity>();
        public List<SpfAuthResultEntity> SpfAuthResults { get; set; } = new List<SpfAuthResultEntity>();
    }
}
