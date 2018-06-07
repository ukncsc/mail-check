using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using Dmarc.DnsRecord.Importer.Lambda.Dns;
using Heijden.DNS;

namespace Dmarc.DnsRecord.Importer.Lambda.RecordProcessor
{
    public class DnsRecordUpdater : IDnsRecordUpdater
    {
        private readonly IDnsRecordClient _dnsRecordClient;

        public DnsRecordUpdater(IDnsRecordClient dnsRecordClient)
        {
            _dnsRecordClient = dnsRecordClient;
        }

        public virtual async Task<List<RecordEntity>> UpdateRecord(Dictionary<DomainEntity, List<RecordEntity>> records)
        {
            List<RecordEntity>[] results = await Task.WhenAll(records.Select(_ => UpdateRecord(_.Key, _.Value)).ToArray());
            return results.SelectMany(_ => _).ToList();
        }

        private async Task<List<RecordEntity>> UpdateRecord(DomainEntity domain, List<RecordEntity> records)
        {
            DnsResponse response = await _dnsRecordClient.GetRecord(domain.Name);

            if (IsSuccess(response.ResponseCode))
            {
                //find old records
                List<RecordEntity> oldRecords = records.Where(existingRecord => response.Records.All(responseRecord => !responseRecord.Equals(existingRecord.RecordInfo)))
                    .Select(_ => new RecordEntity(_.Id, _.Domain, _.RecordInfo, response.ResponseCode, _.FailureCount, DateTime.UtcNow))
                    .ToList();

                //find unchanged records
                List<RecordEntity> unchangedRecords = records.Where(existingRecord => response.Records.Any(responseRecord => responseRecord.Equals(existingRecord.RecordInfo))).ToList();

                //find new records
                List<RecordEntity> newRecords = response.Records.Where(responseRecords => records.All(exisingRecord => !exisingRecord.RecordInfo.Equals(responseRecords)))
                    .Select(_ => new RecordEntity(null, domain, _, response.ResponseCode, 0))
                    .ToList();

                List<RecordEntity> updatedSuccessfulRecords = oldRecords
                    .Concat(unchangedRecords)
                    .Concat(newRecords)
                    .ToList();

                //create placeholder if no records
                if (updatedSuccessfulRecords.All(_ => _.EndDate != null))
                {
                    updatedSuccessfulRecords.Add(new RecordEntity(null, domain, null, response.ResponseCode, 0));
                }

                return updatedSuccessfulRecords;
            }

            //Once placeholder in play keep returning placeholder on failure
            RecordEntity recordEntity = records.SingleOrDefault(_ => _.FailureCount == -1);
            if (recordEntity != null)
            {
                return records;
            }

            //less that 3 failures increment failure count
            if (records.Any() && records.Max(_ => _.FailureCount) < 3)
            {
                return records.Select(_ => new RecordEntity(_.Id, domain, _.RecordInfo, response.ResponseCode, _.FailureCount + 1)).ToList();
            }

            //otherwise expire records and add placeholder
            return records.Select(_ => new RecordEntity(_.Id, domain, _.RecordInfo, response.ResponseCode, _.FailureCount, DateTime.UtcNow)).
                Concat(new List<RecordEntity> { new RecordEntity(null, domain, null, response.ResponseCode, -1) }).ToList();
        }

        private bool IsSuccess(RCode responseCode)
        {
            return responseCode == RCode.NoError || responseCode == RCode.NXDomain;
        }
    }
}
