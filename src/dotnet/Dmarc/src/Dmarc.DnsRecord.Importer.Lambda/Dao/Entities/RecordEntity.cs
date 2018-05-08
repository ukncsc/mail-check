using System;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Heijden.DNS;

namespace Dmarc.DnsRecord.Importer.Lambda.Dao.Entities
{
    public class RecordEntity
    {
        public RecordEntity(
            int? id, 
            DomainEntity domain, 
            RecordInfo recordInfo,
            RCode responseCode, 
            int failureCount, 
            DateTime? endDate = null)
        {
            Id = id;
            Domain = domain;
            RecordInfo = recordInfo;
            ResponseCode = responseCode;
            FailureCount = failureCount;
            EndDate = endDate;
        }

        public int? Id { get; }
        public DomainEntity Domain { get; }
        public RecordInfo RecordInfo { get; }
        public RCode ResponseCode { get; }
        public int FailureCount { get; }
        public DateTime? EndDate { get; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Domain)}: {Domain}, {nameof(RecordInfo)}: {RecordInfo}, {nameof(ResponseCode)}: {ResponseCode}, {nameof(FailureCount)}: {FailureCount}, {nameof(EndDate)}: {EndDate}";
        }
    }
}