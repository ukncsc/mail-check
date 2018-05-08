namespace Dmarc.DomainStatus.Api.Domain
{
    public class DomainsRequest
    {
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public string Search { get; set; } = string.Empty;
    }
}
