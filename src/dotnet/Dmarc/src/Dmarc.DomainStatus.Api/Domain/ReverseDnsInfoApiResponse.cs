using System;
using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class ReverseDnsInfoApiResponse
    {
        public ReverseDnsInfoApiResponse(string ipAddress, DateTime date, IList<DnsResponse> dnsResponses, IList<string> forwardLookupMatches)
        {
            IpAddress = ipAddress;
            Date = date;
            DnsResponses = dnsResponses;
            ForwardLookupMatches = forwardLookupMatches;
        }

        public string IpAddress { get; }

        public DateTime Date { get; }

        public IList<DnsResponse> DnsResponses { get; }

        public IList<string> ForwardLookupMatches { get; }
    }
}
