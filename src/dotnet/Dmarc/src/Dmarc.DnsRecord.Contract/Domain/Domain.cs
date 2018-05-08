namespace Dmarc.DnsRecord.Contract.Domain
{
    public class Domain
    {
        public Domain(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }
    }
}