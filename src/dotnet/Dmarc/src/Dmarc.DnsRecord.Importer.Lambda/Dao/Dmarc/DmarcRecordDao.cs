using System;
using System.Data.Common;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Config;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Heijden.DNS;
using MySql.Data.MySqlClient;

namespace Dmarc.DnsRecord.Importer.Lambda.Dao.Dmarc
{
    public class DmarcRecordDao : DnsRecordDao
    {
        public DmarcRecordDao(IConnectionInfoAsync connectionInfo,
            IRecordImporterConfig recordImporterConfig,
            ILogger log) : base(connectionInfo,
            recordImporterConfig,
            log,
            DmarcRecordDaoResources.SelectDomainsWithRecords,
            DmarcRecordDaoResources.InsertRecord,
            DmarcRecordDaoResources.InsertRecordValueFormatString,
            DmarcRecordDaoResources.InsertRecordOnDuplicateKey)
        {
        }

        protected override Tuple<DomainEntity, RecordEntity> CreateRecordEntity(DbDataReader reader)
        {
            DomainEntity domain = new DomainEntity(reader.GetInt32("domain_id"), reader.GetString("domain_name"));

            int? recordId = reader.GetInt32Nullable("id");

            RecordEntity createRecord = recordId.HasValue ? CreateRecordEntity(reader, recordId, domain) : null;

            return Tuple.Create(domain, createRecord);
        }

        private RecordEntity CreateRecordEntity(DbDataReader reader, int? recordId, DomainEntity domain)
        {
            DmarcRecordInfo dmarcRecordInfo = reader.GetString("record") == null
                ? DmarcRecordInfo.EmptyRecordInfo
                : new DmarcRecordInfo(reader.GetString("record"), reader.GetString("org_domain"),
                    reader.GetBoolean("is_tld"), reader.GetBoolean("is_inherited"));

            return new RecordEntity(
                recordId,
                domain,
                dmarcRecordInfo,
                (RCode) reader.GetInt16("result_code"),
                reader.GetInt16("failure_count"));
        }

        protected override void AddCommandParmeters(MySqlCommand command, RecordEntity record, int index)
        {
            DmarcRecordInfo recordInfo = record.RecordInfo as DmarcRecordInfo;

            command.Parameters.AddWithValue($"a{index}", record.Id);
            command.Parameters.AddWithValue($"b{index}", record.Domain.Id);
            command.Parameters.AddWithValue($"c{index}", recordInfo?.Record);
            command.Parameters.AddWithValue($"d{index}", record.EndDate);
            command.Parameters.AddWithValue($"e{index}", record.FailureCount);
            command.Parameters.AddWithValue($"f{index}", (ushort) record.ResponseCode);
            command.Parameters.AddWithValue($"g{index}", recordInfo?.OrgDomain);
            command.Parameters.AddWithValue($"h{index}", recordInfo?.IsTld != null && recordInfo.IsTld);
            command.Parameters.AddWithValue($"i{index}", recordInfo?.IsInherited != null && recordInfo.IsInherited);
        }
    }
}
