using System.Collections.Generic;
using Dmarc.DnsRecord.Contract.Domain;

namespace Dmarc.DnsRecord.Contract.Messages
{
    public class SpfConfigsUpdated : DnsRecordMessage
    {
        public SpfConfigsUpdated(List<SpfConfig> spfConfigs)
        {
            SpfConfigs = spfConfigs;
        }

        public List<SpfConfig> SpfConfigs { get; }
    }
}

