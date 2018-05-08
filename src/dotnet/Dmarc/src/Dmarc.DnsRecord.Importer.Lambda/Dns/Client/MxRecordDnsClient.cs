using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Dmarc.DnsRecord.Importer.Lambda.Util;
using Heijden.DNS;

namespace Dmarc.DnsRecord.Importer.Lambda.Dns.Client
{
    public class MxRecordDnsClient : DnsRecordClient
    {
        public MxRecordDnsClient(IDnsResolver dnsResolver, ILogger log) 
            : base(dnsResolver, log, QType.MX, "MX"){ }

        protected override List<RecordInfo> GetRecords(Response response)
        {
            List<RecordInfo> records = response.RecordsMX
                .Select(_ => new MxRecordInfo(_.EXCHANGE.EscapeNonAsciiChars(), _.PREFERENCE))
                .Cast<RecordInfo>()
                .ToList();

            return records.Any() ? records : new List<RecordInfo> {MxRecordInfo.EmptyRecordInfo};
        }
    }
}