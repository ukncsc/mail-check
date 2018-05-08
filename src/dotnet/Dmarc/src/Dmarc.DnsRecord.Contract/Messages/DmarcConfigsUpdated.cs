using System.Collections.Generic;
using Dmarc.DnsRecord.Contract.Domain;

namespace Dmarc.DnsRecord.Contract.Messages
{
    public class DmarcConfigsUpdated : DnsRecordMessage
    {
        public DmarcConfigsUpdated(List<DmarcConfig> dmarcConfigs)
        {
            DmarcConfigs = dmarcConfigs;
        }

        public List<DmarcConfig> DmarcConfigs { get; }
    }
}