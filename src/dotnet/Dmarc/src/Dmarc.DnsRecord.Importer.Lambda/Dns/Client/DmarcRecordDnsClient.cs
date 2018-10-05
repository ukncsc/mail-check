using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Dmarc.DnsRecord.Importer.Lambda.Util;
using Heijden.DNS;

namespace Dmarc.DnsRecord.Importer.Lambda.Dns.Client
{
    public class DmarcRecordDnsClient : DnsRecordClient
    {
        private readonly IOrganisationalDomainProvider _organisationalDomainProvider;

        public DmarcRecordDnsClient(IDnsResolver dnsResolver, ILogger log,
            IOrganisationalDomainProvider organisationalDomainProvider)
            : base(dnsResolver, log, QType.TXT, "DMARC")
        {
            _organisationalDomainProvider = organisationalDomainProvider;
        }

        protected override List<RecordInfo> GetRecords(Response response)
        {
            return GetDmarcRecords(response);
        }

        protected override string FormatQuery(string domain)
        {
            return $"_dmarc.{domain}";
        }

        public override async Task<DnsResponse> GetRecord(string domain)
        {
            Response response = await _dnsResolver.GetRecord(FormatQuery(domain), _recordType);

            OrganisationalDomain organisationalDomain =
                await _organisationalDomainProvider.GetOrganisationalDomain(domain);

            string orgDomain = organisationalDomain.OrgDomain;
            
            bool isTld = organisationalDomain.IsTld;

            List<RecordInfo> dnsRecords = GetDmarcRecords(response, orgDomain, isTld);

            if (!dnsRecords.Any())
            {
                if (!organisationalDomain.IsOrgDomain && !organisationalDomain.IsTld)
                {
                    response = await _dnsResolver.GetRecord(FormatQuery(orgDomain), _recordType);
                    dnsRecords = GetDmarcRecords(response, orgDomain, isTld, true);
                }
            }

            if (response.header.RCODE == RCode.NoError || response.header.RCODE == RCode.NXDomain)
            {
                string records = string.Join(Environment.NewLine, dnsRecords);
                _log.Trace($"Found following {_recordName} records for {domain}: {Environment.NewLine}{records}");
            }
            else
            {
                _log.Error($"Failed to retrieve {_recordName} records with RCODE: {response.header.RCODE}");
            }

            return new DnsResponse(
                !dnsRecords.Any() ? new List<RecordInfo> {DmarcRecordInfo.EmptyRecordInfo} : dnsRecords,
                response.header.RCODE);
        }

        private List<RecordInfo> GetDmarcRecords(Response response, string orgDomain = null, bool isTld = false, bool isInherited = false)
        {
            return response.RecordsTXT
                .Where(_ => _.TXT.FirstOrDefault()?.StartsWith("v=dmarc", StringComparison.OrdinalIgnoreCase) ?? false)
                .Select(_ => CreateRecordInfo(_, orgDomain, isTld, isInherited))
                .ToList();
        }

        private RecordInfo CreateRecordInfo(RecordTXT recordTxt, string orgDomain, bool isTld, bool isInherited)
        {
            var record = string.Join(string.Empty, recordTxt.TXT);
            return new DmarcRecordInfo(record.EscapeNonAsciiChars(), orgDomain, isTld, isInherited);
        }
    }
}
