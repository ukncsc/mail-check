namespace Dmarc.DnsRecord.Evaluator.Seeding.Dao.Entities
{
    public class Domain
    {
        public Domain(int domainId, string domainName)
        {
            DomainId = domainId;
            DomainName = domainName;
        }

        public int DomainId { get; }
        public string DomainName { get; }
    }
}