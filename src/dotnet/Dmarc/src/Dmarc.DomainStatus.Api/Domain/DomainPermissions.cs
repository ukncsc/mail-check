namespace Dmarc.DomainStatus.Api.Domain
{
    public class DomainPermissions
    {
        public int DomainId { get; }
        public bool AggregatePermission { get; }
        public bool DomainPermission { get; }

        public DomainPermissions(int domainId, bool aggregatePermission, bool domainPermission)
        {
            DomainId = domainId;
            AggregatePermission = aggregatePermission;
            DomainPermission = domainPermission;
        }
    }
}