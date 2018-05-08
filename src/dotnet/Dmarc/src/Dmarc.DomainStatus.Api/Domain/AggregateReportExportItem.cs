using System;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class AggregateReportExportItem
    {
        public AggregateReportExportItem(string headerFrom, string sourceIp, string ptr, int count, string spf, string dkim, string disposition, string orgName, DateTime effectiveDate)
        {
            HeaderFrom = headerFrom;
            SourceIp = sourceIp;
            Ptr = ptr;
            Count = count;
            Spf = spf;
            Dkim = dkim;
            Disposition = disposition;
            OrgName = orgName;
            EffectiveDate = effectiveDate;
        }

        public string HeaderFrom { get; }
        public string SourceIp { get; }
        public string Ptr { get; }
        public int Count { get; }
        public string Spf { get; }
        public string Dkim { get; }
        public string Disposition { get; }
        public string OrgName { get; }
        public DateTime EffectiveDate { get; }
    }
}