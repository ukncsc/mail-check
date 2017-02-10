namespace Dmarc.AggregateReport.Api.Messages
{
    internal class DomainSearchRequest : Request
    {
        public DomainSearchRequest(string searchPattern)
        {
            SearchPattern = searchPattern;
        }

        public string SearchPattern { get; }
    }
}
