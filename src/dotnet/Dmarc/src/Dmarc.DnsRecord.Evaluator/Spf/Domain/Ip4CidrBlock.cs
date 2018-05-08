namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public class Ip4CidrBlock : SpfEntity
    {
        public Ip4CidrBlock(int? value)
        {
            Value = value;
        }

        public int? Value { get; }
    }
}