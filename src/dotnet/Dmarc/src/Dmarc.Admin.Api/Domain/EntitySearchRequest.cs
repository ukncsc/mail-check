using System.Collections.Generic;

namespace Dmarc.Admin.Api.Domain
{
    public class EntitySearchRequest
    {
        public string Search { get; set; } = string.Empty;
        public int Limit { get; set; } = 10;
        public List<int> IncludedIds { get; set; } = new List<int>();
    }
}