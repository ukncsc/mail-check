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
    public class DmarcRecordDnsClient : DnsRecordClient
    {
        public DmarcRecordDnsClient(IDnsResolver dnsResolver, ILogger log) 
            : base(dnsResolver, log, QType.TXT, "DMARC"){}

        protected override List<RecordInfo> GetRecords(Response response)
        {
            List<RecordInfo> records = response.RecordsTXT
                .Where(_ => _.TXT.FirstOrDefault()?.StartsWith("v=dmarc", StringComparison.OrdinalIgnoreCase) ?? false)
                .Select(CreateRecordInfo)
                .ToList();

            return records.Any() ? records : new List<RecordInfo> {DmarcRecordInfo.EmptyRecordInfo};
        }

        protected override string FormatQuery(string domain)
        {
            return $"_dmarc.{domain}";
        }


        private RecordInfo CreateRecordInfo(RecordTXT recordTxt)
        {
            var record = string.Join(string.Empty, recordTxt.TXT);
            return new DmarcRecordInfo(record.EscapeNonAsciiChars());
        }
    }
}
