using System.Collections.Generic;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Heijden.DNS;

namespace Dmarc.DnsRecord.Importer.Lambda.Dns
{
    public class DnsResponse
    {
        public DnsResponse(List<RecordInfo> records, RCode responseCode)
        {
            Records = records;
            ResponseCode = responseCode;
        }

        public List<RecordInfo> Records { get; }

        public RCode ResponseCode { get; }
    }
}