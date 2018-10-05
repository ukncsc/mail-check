using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Heijden.DNS;

namespace Dmarc.DnsRecord.Importer.Lambda.Dns.Client
{
    public abstract class DnsRecordClient : IDnsRecordClient
    {
        protected readonly IDnsResolver _dnsResolver;
        protected readonly ILogger _log;
        protected readonly QType _recordType;
        protected readonly string _recordName;

        protected DnsRecordClient(
            IDnsResolver dnsResolver, 
            ILogger log, 
            QType recordType, 
            string recordName)
        {
            _dnsResolver = dnsResolver;
            _log = log;
            _recordType = recordType;
            _recordName = recordName;
        }

        public virtual async Task<DnsResponse> GetRecord(string domain)
        {
            Response response = await _dnsResolver.GetRecord(FormatQuery(domain), _recordType);

            List<RecordInfo> dnsRecords = GetRecords(response);
            if (response.header.RCODE == RCode.NoError || response.header.RCODE == RCode.NXDomain)
            {
                string records = string.Join(Environment.NewLine, dnsRecords);
                _log.Trace($"Found following { _recordName } records for {domain}: {Environment.NewLine}{records}");
            }
            else
            {
                _log.Error($"Failed to retrieve { _recordName } records with RCODE: {response.header.RCODE}");
            }

            return new DnsResponse(dnsRecords, response.header.RCODE);
        }

        protected abstract List<RecordInfo> GetRecords(Response response);

        protected virtual string FormatQuery(string domain)
        {
            return domain;
        }
    }
}