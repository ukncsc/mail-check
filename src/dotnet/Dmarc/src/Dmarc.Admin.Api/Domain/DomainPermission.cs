using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dmarc.Admin.Api.Domain
{
    public class DomainPermission
    {
        public DomainPermission(Domain domain, List<Permission> permissions)
        {
            Domain = domain;
            Permissions = permissions;
        }

        public Domain Domain { get; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public List<Permission> Permissions { get; }
    }
}