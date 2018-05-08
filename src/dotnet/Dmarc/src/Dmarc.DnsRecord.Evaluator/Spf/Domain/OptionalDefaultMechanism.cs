namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public abstract class OptionalDefaultMechanism : Mechanism
    {
        protected OptionalDefaultMechanism(string value, Qualifier qualifier, bool isImplicit) 
            : base(value, qualifier)
        {
            IsImplicit = isImplicit;
        }

        public bool IsImplicit { get; }
    }
}