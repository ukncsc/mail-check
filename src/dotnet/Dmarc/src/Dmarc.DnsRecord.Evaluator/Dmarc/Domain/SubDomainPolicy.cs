namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class SubDomainPolicy : OptionalDefaultTag
    {
        public SubDomainPolicy(string value, PolicyType policyType, bool isImplicit=false) : base(value, isImplicit)
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