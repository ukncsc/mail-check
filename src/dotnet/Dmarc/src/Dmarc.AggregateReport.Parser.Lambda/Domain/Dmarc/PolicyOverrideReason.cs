namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class PolicyOverrideReason
    {
        public PolicyOverrideReason()
        {
        }

        public PolicyOverrideReason(PolicyOverride? policyOverride, string comment)
        {
            PolicyOverride = policyOverride;
            Comment = comment;
        }

        public PolicyOverride? PolicyOverride { get; set; }

        public string Comment { get; set; }
    }
}