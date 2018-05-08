namespace Dmarc.DomainStatus.Api.Domain
{
    public class AggregateSummaryItem
    {
        public AggregateSummaryItem(int fullyTrusted, int partiallyTrusted, int untrusted, int quarantined, int rejected)
        {
            FullyTrusted = fullyTrusted;
            PartiallyTrusted = partiallyTrusted;
            Untrusted = untrusted;
            Quarantined = quarantined;
            Rejected = rejected;
        }

        public int FullyTrusted { get; }
        public int PartiallyTrusted { get; }
        public int Untrusted { get; }
        public int Quarantined { get; }
        public int Rejected { get; }
    }
}