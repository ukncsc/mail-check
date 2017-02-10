using System.Collections.Generic;

namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal class Record
    {
        public long Id { get; set; }
        public long AggregateReportId { get; set; }
        public string SourceIp { get; set; }
        public int Count { get; set; }
        public Disposition? Disposition { get; set; }
        public DmarcResult? Dkim { get; set; }
        public DmarcResult Spf { get; set; }
        public List<PolicyOverrideReason> Reason { get; set; } = new List<PolicyOverrideReason>();
        public string EnvelopeTo { get; set; }
        public string HeaderFrom { get; set; }
        public List<DkimAuthResult> DkimAuthResults { get; set; } = new List<DkimAuthResult>();
        public List<SpfAuthResult> SpfAuthResults { get; set; } = new List<SpfAuthResult>();
    }
}