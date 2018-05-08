using System;
using System.Data.Common;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Config;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Heijden.DNS;
using MySql.Data.MySqlClient;

namespace Dmarc.DnsRecord.Importer.Lambda.Dao.Spf
{
    public class SpfRecordDao : DnsRecordDao
    {
        public SpfRecordDao(IConnectionInfoAsync connectionInfo,
            IRecordImporterConfig recordImporterConfig,
            ILogger log)
            : base(connectionInfo,
                  recordImporterConfig,
                  log,
                  SpfRecordDaoResources.SelectDomainsWithRecords,
                  SpfRecordDaoResources.InsertRecord,
                  SpfRecordDaoResources.InsertRecordValueFormatString,
                  SpfRecordDaoResources.InsertRecordOnDuplicateKey)
        {
        }

        protected override Tuple<DomainEntity, RecordEntity> CreateRecordEntity(DbDataReader reader)
        {
            DomainEntity domain = new DomainEntity(reader.GetInt32("domain_id"), reader.GetString("domain_name"));

            int? recordId = reader.GetInt32Nullable("id");

            RecordEntity createRecord = recordId.HasValue ?
                CreateRecordEntity(reader, recordId, domain) :
                null;

            return Tuple.Create(domain, createRecord);
        }

        private RecordEntity CreateRecordEntity(DbDataReader reader, int? recordId, DomainEntity domain)
        {
            SpfRecordInfo spfRecordInfo = reader.GetString("record") == null
                ? SpfRecordInfo.EmptyRecordInfo
                : new SpfRecordInfo(reader.GetString("record"));

            return new RecordEntity(
                recordId.Value,
                domain,
                spfRecordInfo,
                (RCode)reader.GetInt16("result_code"),
                reader.GetInt16("failure_count"));
        }

        protected override void AddCommandParmeters(MySqlCommand command, RecordEntity record, int index)
        {
            SpfRecordInfo recordInfo = record.RecordInfo as SpfRecordInfo;

            command.Parameters.AddWithValue($"a{index}", record.Id);
            command.Parameters.AddWithValue($"b{index}", record.Domain.Id);
            command.Parameters.AddWithValue($"c{index}", recordInfo?.Record);
            command.Parameters.AddWithValue($"d{index}", record.EndDate);
            command.Parameters.AddWithValue($"e{index}", record.FailureCount);
            command.Parameters.AddWithValue($"f{index}", (ushort)record.ResponseCode);
        }
    }
}
