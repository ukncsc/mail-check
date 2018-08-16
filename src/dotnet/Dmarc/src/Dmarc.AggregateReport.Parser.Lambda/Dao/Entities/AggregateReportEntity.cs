using System;
using System.Collections.Generic;

namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal class AggregateReportEntity
    {
        public long Id { get; set; }
        public string RequestId { get; set; }
        public string OrginalUri { get; set; }
        public string AttachmentFilename { get; set; }
        public double? Version { get; set; }
        public string OrgName { get; set; }
        public string Email { get; set; }    
        public string ReportId { get; set; }
        public string ExtraContactInfo { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Domain { get; set; }
        public EntityAlignment? Adkim { get; set; }
        public EntityAlignment? Aspf { get; set; }
        public EntityDisposition P { get; set; }
        public EntityDisposition? Sp { get; set; }
        public int? Pct { get; set; }
        public string Fo { get; set; }
        public List<RecordEntity> Records { get; set; } = new List<RecordEntity>();
    }
}
