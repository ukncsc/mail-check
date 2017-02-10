using System;
using System.Collections.Generic;

namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal class AggregateReport
    {
        public long Id { get; set; }
        public string RequestId { get; set; }
        public string OrginalUri { get; set; }
        public string AttachmentFilename { get; set; }
        public string OrgName { get; set; }
        public string Email { get; set; }    
        public string ReportId { get; set; }
        public string ExtraContactInfo { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Domain { get; set; }
        public Alignment? Adkim { get; set; }
        public Alignment? Aspf { get; set; }
        public Disposition P { get; set; }
        public Disposition? Sp { get; set; }
        public int? Pct { get; set; }
        public List<Record> Records { get; set; } = new List<Record>();
    }
}
