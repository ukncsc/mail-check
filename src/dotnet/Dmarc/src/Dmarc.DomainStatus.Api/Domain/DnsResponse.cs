using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class DnsResponse
    {
        public DnsResponse(string host, IList<string> ipAddresses)
        {
            Host = host;
            IpAddresses = ipAddresses ?? new List<string>();
        }

        public string Host { get; }

        public IList<string> IpAddresses { get; }
    }
}
