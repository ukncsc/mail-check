using System;
using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class ReverseDnsInfoApiRequest
    {
        public ReverseDnsInfoApiRequest(List<string> ipAddresses, DateTime date)
        {
            IpAddresses = IpAddresses;
            Date = date;
        }

        private List<string> IpAddresses { get; }

        private DateTime Date { get; }
    }
}
