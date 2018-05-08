namespace Dmarc.Admin.Api.Domain
{
    public class GetEntitiesByRelatedIdRequest
    {
        public int Id { get; set; }
        public string Search { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}