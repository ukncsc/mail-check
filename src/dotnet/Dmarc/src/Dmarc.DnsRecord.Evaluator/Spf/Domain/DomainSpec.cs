namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public class DomainSpec : SpfEntity
    {
        public DomainSpec(string domain)
        {
            Domain = domain;
        }

        public string Domain { get; }

        public override string ToString()
        {
            return $"{nameof(Domain)}: {Domain}";
        }
    }
}