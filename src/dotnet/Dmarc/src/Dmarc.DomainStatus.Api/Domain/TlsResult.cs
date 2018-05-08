using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class TlsResult
    {
        public TlsResult(bool supported, CipherSuite? cipherSuite)
        {
            Supported = supported;
            CipherSuite = cipherSuite;
        }

        public bool Supported { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CipherSuite? CipherSuite { get; }
    }
}