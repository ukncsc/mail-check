namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public class Ip6CidrBlock : SpfEntity
    {
        public Ip6CidrBlock(int? value)
        {
            Value = value;
        }

        public int? Value { get; }
    }
}