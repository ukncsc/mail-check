using System;
using System.Data.Common;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Config;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Heijden.DNS;
using MySql.Data.MySqlClient;

namespace Dmarc.DnsRecord.Importer.Lambda.Dao.Mx
{
    public class MxRecordDao : DnsRecordDao
    {
        public MxRecordDao(IConnectionInfoAsync connectionInfo,
            IRecordImporterConfig recordImporterConfig,
            ILogger log)
            : base(connectionInfo,
                  recordImporterConfig,
                  log,
                  MxRecordDaoResources.SelectDomainsWithRecords,
                  MxRecordDaoResources.InsertRecord,
                  MxRecordDaoResources.InsertRecordValueFormatString,
                  MxRecordDaoResources.InsertRecordOnDuplicateKey)
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
            int? preference = reader.GetInt32Nullable("preference");
            string hostname = reader.GetString("hostname");

            MxRecordInfo mxRecordInfo = preference == null || hostname == null
                ? MxRecordInfo.EmptyRecordInfo :
                new MxRecordInfo(hostname, preference.Value);

            return new RecordEntity(
                recordId.Value,
                domain,
                mxRecordInfo,
                (RCode)reader.GetInt16("result_code"),
                reader.GetInt16("failure_count"));
        }

        protected override void AddCommandParmeters(MySqlCommand command, RecordEntity record, int index)
        {
            MxRecordInfo recordInfo = record.RecordInfo as MxRecordInfo;

            command.Parameters.AddWithValue($"a{index}", record.Id);
            command.Parameters.AddWithValue($"b{index}", record.Domain.Id);
            command.Parameters.AddWithValue($"c{index}", recordInfo?.Preference);
            command.Parameters.AddWithValue($"d{index}", recordInfo?.Host);
            command.Parameters.AddWithValue($"e{index}", record.EndDate);
            command.Parameters.AddWithValue($"f{index}", record.FailureCount);
            command.Parameters.AddWithValue($"g{index}", (ushort)record.ResponseCode);
        }
    }
}
