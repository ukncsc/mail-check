using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class DomainSecurityInfo
    {
        public DomainSecurityInfo(Domain domain, 
            bool hasDmarc,
            Status tlsStatus,
            Status dmarcStatus,
            Status spfStatus)
        {
            Domain = domain;
            HasDmarc = hasDmarc;
            TlsStatus = tlsStatus;
            DmarcStatus = dmarcStatus;
            SpfStatus = spfStatus;
        }

        public Domain Domain { get; }

        [JsonIgnore]
        public bool HasDmarc { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Status TlsStatus { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Status DmarcStatus { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Status SpfStatus { get; }
    }
}