namespace Dmarc.Admin.Api.Domain
{
    public class AllEntitiesSearchRequest
    {
        public string Search { get; set; } = string.Empty;
        public int Limit { get; set; } = 10;
    }
}