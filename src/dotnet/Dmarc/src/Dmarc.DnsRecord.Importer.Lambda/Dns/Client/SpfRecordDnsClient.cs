using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Dmarc.DnsRecord.Importer.Lambda.Util;
using Heijden.DNS;

namespace Dmarc.DnsRecord.Importer.Lambda.Dns.Client
{
    public class SpfRecordDnsClient : DnsRecordClient
    {
        public SpfRecordDnsClient(IDnsResolver dnsResolver, ILogger log)
            : base(dnsResolver, log, QType.TXT, "SPF") { }

        protected override List<RecordInfo> GetRecords(Response response)
        {
            List<RecordInfo> records = response.RecordsTXT
                .Where(_ => _.TXT.FirstOrDefault()?.StartsWith("v=spf1", StringComparison.OrdinalIgnoreCase) ?? false)
                .Select(CreateRecordInfo)
                .ToList();

            return records.Any() ? records : new List<RecordInfo> {SpfRecordInfo.EmptyRecordInfo};
        }

        private RecordInfo CreateRecordInfo(RecordTXT recordTxt)
        {
            var record = string.Join(string.Empty, recordTxt.TXT);
            return new SpfRecordInfo(record.EscapeNonAsciiChars());
        }
    }
}