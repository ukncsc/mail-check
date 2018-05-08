using System;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;

namespace Dmarc.AggregateReport.Parser.Lambda.Domain
{
    public class DenormalisedRecord
    {
        public DenormalisedRecord(string orginalUri,
            string orgName,
            string email,
            string extraContactInfo,
            DateTime beginDate,
            DateTime endDate,
            string domain,
            Alignment? adkim,
            Alignment? aspf,
            Disposition p,
            Disposition? sp,
            int? pct,
            string sourceIp,
            int count,
            Disposition? disposition,
            DmarcResult? dkim,
            DmarcResult? spf,
            string reason,
            string comment,
            string envelopeTo,
            string headerFrom,
            string dkimDomain,
            string dkimResult,
            string dkimHumanResult,
            string spfDomain,
            string spfResult)
        {
            OrginalUri = orginalUri;
            OrgName = orgName;
            Email = email;
            ExtraContactInfo = extraContactInfo;
            BeginDate = beginDate;
            EndDate = endDate;
            Domain = domain;
            Adkim = adkim;
            Aspf = aspf;
            P = p;
            Sp = sp;
            Pct = pct;
            SourceIp = sourceIp;
            Count = count;
            Disposition = disposition;
            Dkim = dkim;
            Spf = spf;
            Reason = reason;
            Comment = comment;
            EnvelopeTo = envelopeTo;
            HeaderFrom = headerFrom;
            DkimDomain = dkimDomain;
            DkimResult = dkimResult;
            DkimHumanResult = dkimHumanResult;
            SpfDomain = spfDomain;
            SpfResult = spfResult;
        }

        public string OrginalUri { get; }
        public string OrgName { get; }
        public string Email { get; }
        public string ExtraContactInfo { get; }
        public DateTime BeginDate { get; }
        public DateTime EndDate { get; }
        public string Domain { get; }
        public Alignment? Adkim { get; }
        public Alignment? Aspf { get; }
        public Disposition P { get; }
        public Disposition? Sp { get; }
        public int? Pct { get; }
        public string SourceIp { get; }
        public int Count { get; }
        public Disposition? Disposition { get; }
        public DmarcResult? Dkim { get; }
        public DmarcResult? Spf { get; }
        public string Reason { get; }
        public string Comment { get; }
        public string EnvelopeTo { get; }
        public string HeaderFrom { get; }
        public string DkimDomain { get; }
        public string DkimResult { get; }
        public string DkimHumanResult { get; }
        public string SpfDomain { get; }
        public string SpfResult { get; }
    }
}
