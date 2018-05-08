namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public abstract class Modifier : Term
    {
        protected Modifier(string value)
            : base(value){}
    }
}