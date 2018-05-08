namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class Policy : Tag
    {
        public Policy(string value, PolicyType policyType) : base(value)
        {
            PolicyType = policyType;
        }

        public PolicyType PolicyType { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(PolicyType)}: {PolicyType}";
        }
    }
}