namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class PolicyEvaluated
    {
        public PolicyEvaluated()
        {
        }

        public PolicyEvaluated(Disposition? disposition, DmarcResult? dkim, DmarcResult? spf, PolicyOverrideReason[] reasons)
        {
            Disposition = disposition;
            Dkim = dkim;
            Spf = spf;
            Reasons = reasons;
        }

        public Disposition? Disposition { get; set; }

        public DmarcResult? Dkim { get; set; }

        public DmarcResult? Spf { get; set; }

        public PolicyOverrideReason[] Reasons { get; set; }
    }
}
