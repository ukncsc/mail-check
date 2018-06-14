using System;
using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class ReverseDnsInfoApiRequest
    {
        public ReverseDnsInfoApiRequest(List<string> ipAddresses, DateTime date)
        {
            IpAddresses = ipAddresses;
            Date = date;
        }

        public List<string> IpAddresses { get; }

        public DateTime Date { get; }
    }
}
