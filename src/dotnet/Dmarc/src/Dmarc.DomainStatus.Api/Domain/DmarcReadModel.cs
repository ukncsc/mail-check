namespace Dmarc.DomainStatus.Api.Domain
{
    public class DmarcReadModel
    {
        public DmarcReadModel(Domain domain, bool hasDmarc, string model)
        {
            Domain = domain;
            HasDmarc = hasDmarc;
            Model = model;
        }

        public Domain Domain { get; }
        public bool HasDmarc { get; }
        public string Model { get; }
    }
}
