using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Parser.Lambda.Dao.Entities;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Linq;
using Dmarc.Common.Report.Persistance.Dao;
using MySql.Data.MySqlClient;

namespace Dmarc.AggregateReport.Parser.Lambda.Dao
{
    internal class AggregateReportParserDaoAsync : IReportDaoAsync<AggregateReportEntity>
    {
        private const int BatchSize = 10000;

        private readonly IConnectionInfoAsync _connectionInfoAsync;
        private readonly ILogger _log;

        public AggregateReportParserDaoAsync(IConnectionInfoAsync connectionInfoAsync, ILogger log)
        {
            _connectionInfoAsync = connectionInfoAsync;
            _log = log;
        }

        #region Aggregate Report

        public async Task<bool> Add(AggregateReportEntity aggregateReportEntity)
        {
            bool added = false;
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfoAsync.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
      
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    MySqlCommand command = new MySqlCommand(AggregateReportParserDaoResources.InsertAggregateReport,
                        connection, transaction);
                    command.Parameters.AddWithValue("request_id", aggregateReportEntity.RequestId);
                    command.Parameters.AddWithValue("original_uri", aggregateReportEntity.OrginalUri);
                    command.Parameters.AddWithValue("attachment_filename", aggregateReportEntity.AttachmentFilename);
                    command.Parameters.AddWithValue("version", aggregateReportEntity.Version);
                    command.Parameters.AddWithValue("org_name", aggregateReportEntity.OrgName);
                    command.Parameters.AddWithValue("email", aggregateReportEntity.Email);
                    command.Parameters.AddWithValue("report_id", aggregateReportEntity.ReportId);
                    command.Parameters.AddWithValue("extra_contact_info", aggregateReportEntity.ExtraContactInfo);
                    command.Parameters.AddWithValue("effective_date", aggregateReportEntity.EffectiveDate);
                    command.Parameters.AddWithValue("begin_date", aggregateReportEntity.BeginDate);
                    command.Parameters.AddWithValue("end_date", aggregateReportEntity.EndDate);
                    command.Parameters.AddWithValue("domain", aggregateReportEntity.Domain);
                    command.Parameters.AddWithValue("adkim", aggregateReportEntity.Adkim?.ToString());
                    command.Parameters.AddWithValue("aspf", aggregateReportEntity.Aspf?.ToString());
                    command.Parameters.AddWithValue("p", aggregateReportEntity.P.ToString());
                    command.Parameters.AddWithValue("sp", aggregateReportEntity.Sp?.ToString());
                    command.Parameters.AddWithValue("pct", aggregateReportEntity.Pct);
                    command.Parameters.AddWithValue("fo", aggregateReportEntity.Fo);
                    command.Parameters.AddWithValue("created_date", DateTime.UtcNow);
                    int numberOfUpdates = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                    if (numberOfUpdates != 0) // number of updated == 0 -> duplicate report id.             
                    {
                        aggregateReportEntity.Id = command.LastInsertedId;

                        foreach (var record in aggregateReportEntity.Records)
                        {
                            record.AggregateReportId = aggregateReportEntity.Id;
                        }

                        await AddRecords(aggregateReportEntity.Records.ToArray(), connection, transaction).ConfigureAwait(false);
                        added = true;
                    }

                    await transaction.CommitAsync().ConfigureAwait(false);
                    connection.Close();
                }
            }
            return added;
        }

        #endregion Aggregate Report

        #region RecordEntity

        private async Task AddRecords(RecordEntity[] recordsEntity, MySqlConnection connection, MySqlTransaction transaction)
        {
            foreach (var batch in recordsEntity.Batch(BatchSize))
            {
                RecordEntity[] recordEntityBatch = batch.ToArray();
                MySqlCommand command = new MySqlCommand(connection, transaction);

                StringBuilder stringBuilder = new StringBuilder(AggregateReportParserDaoResources.InsertRecord);
                for (int i = 0; i < recordEntityBatch.Length; i++)
                {
                    stringBuilder.Append(string.Format(AggregateReportParserDaoResources.InsertRecordValueFormatString, i));
                    stringBuilder.Append(i < recordEntityBatch.Length - 1 ? "," : ";");

                    command.Parameters.AddWithValue($"a{i}", recordEntityBatch[i].AggregateReportId);
                    command.Parameters.AddWithValue($"b{i}", recordEntityBatch[i].SourceIp);
                    command.Parameters.AddWithValue($"c{i}", recordEntityBatch[i].Count);
                    command.Parameters.AddWithValue($"d{i}", recordEntityBatch[i].Disposition?.ToString());
                    command.Parameters.AddWithValue($"e{i}", recordEntityBatch[i].Dkim?.ToString());
                    command.Parameters.AddWithValue($"f{i}", recordEntityBatch[i].Spf.ToString());
                    command.Parameters.AddWithValue($"g{i}", recordEntityBatch[i].EnvelopeTo);
                    command.Parameters.AddWithValue($"h{i}", recordEntityBatch[i].EnvelopeFrom);
                    command.Parameters.AddWithValue($"i{i}", recordEntityBatch[i].HeaderFrom);
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                long lastInsertId = command.LastInsertedId;
                foreach (RecordEntity record in recordEntityBatch)
                {
                    foreach (var reason in record.Reason)
                    {
                        reason.RecordId = lastInsertId;
                    }

                    foreach (var dkimAuthResult in record.DkimAuthResults)
                    {
                        dkimAuthResult.RecordId = lastInsertId;
                    }

                    foreach (var spfAuthResult in record.SpfAuthResults)
                    {
                        spfAuthResult.RecordId = lastInsertId;
                    }
                    lastInsertId++;
                }
            }

            await AddPolicyOverrideReasons(recordsEntity.SelectMany(_ => _.Reason).ToArray(), connection, transaction).ConfigureAwait(false);
            await AddDkimAuthResults(recordsEntity.SelectMany(_ => _.DkimAuthResults).ToArray(), connection, transaction).ConfigureAwait(false);
            await AddSpfAuthResults(recordsEntity.SelectMany(_ => _.SpfAuthResults).ToArray(), connection, transaction).ConfigureAwait(false);
        }
        #endregion RecordEntity

        #region PolicyOverrideReasonEntity

        private async Task AddPolicyOverrideReasons(PolicyOverrideReasonEntity[] reasonsEntity, MySqlConnection connection,
            MySqlTransaction transaction)
        {
            foreach (var batch in reasonsEntity.Batch(BatchSize))
            {
                PolicyOverrideReasonEntity[] reasonsEntityBatch = batch.ToArray();
                MySqlCommand command = new MySqlCommand(connection, transaction);
                command.Transaction = transaction;

                StringBuilder stringBuilder = new StringBuilder(AggregateReportParserDaoResources.InsertPolicyOverrideReason);

                for (int i = 0; i < reasonsEntityBatch.Length; i++)
                {
                    stringBuilder.Append(
                        string.Format(AggregateReportParserDaoResources.InsertPolicyOverrideReasonValueFormatString, i));
                    stringBuilder.Append(i < reasonsEntityBatch.Length - 1 ? "," : ";");

                    command.Parameters.AddWithValue($"a{i}", reasonsEntityBatch[i].RecordId);
                    command.Parameters.AddWithValue($"b{i}", reasonsEntityBatch[i].PolicyOverride?.ToString());
                    command.Parameters.AddWithValue($"c{i}", reasonsEntityBatch[i].Comment);
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region DkimAuthResults

        private async Task AddDkimAuthResults(DkimAuthResultEntity[] dkimAuthResultsEntity, MySqlConnection connection,
            MySqlTransaction transaction)
        {
            foreach (var batch in dkimAuthResultsEntity.Batch(BatchSize))
            {
                DkimAuthResultEntity[] dkimAuthResultEntityBatch = batch.ToArray();
                MySqlCommand command = new MySqlCommand(connection, transaction);
                command.Transaction = transaction;

                StringBuilder stringBuilder = new StringBuilder(AggregateReportParserDaoResources.InsertDkimAuthResult);

                for (int i = 0; i < dkimAuthResultEntityBatch.Length; i++)
                {
                    stringBuilder.Append(string.Format(
                        AggregateReportParserDaoResources.InsertDkimAuthResultValueFormatString, i));
                    stringBuilder.Append(i < dkimAuthResultEntityBatch.Length - 1 ? "," : ";");

                    command.Parameters.AddWithValue($"a{i}", dkimAuthResultEntityBatch[i].RecordId);
                    command.Parameters.AddWithValue($"b{i}", dkimAuthResultEntityBatch[i].Domain);
                    command.Parameters.AddWithValue($"c{i}", dkimAuthResultEntityBatch[i].Selector);
                    command.Parameters.AddWithValue($"d{i}", dkimAuthResultEntityBatch[i].Result?.ToString());
                    command.Parameters.AddWithValue($"e{i}", dkimAuthResultEntityBatch[i].HumanResult);
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region SpfAuthResults

        private async Task AddSpfAuthResults(SpfAuthResultEntity[] spfAuthResultsEntity, MySqlConnection connection,
            MySqlTransaction transaction)
        {
            foreach (var batch in spfAuthResultsEntity.Batch(BatchSize))
            {
                SpfAuthResultEntity[] spfAuthResultEntityBatch = batch.ToArray();
                MySqlCommand command = new MySqlCommand(connection, transaction);
                command.Transaction = transaction;

                StringBuilder stringBuilder = new StringBuilder(AggregateReportParserDaoResources.InsertSpfAuthResult);

                for (int i = 0; i < spfAuthResultEntityBatch.Length; i++)
                {
                    stringBuilder.Append(string.Format(
                        AggregateReportParserDaoResources.InsertSpfAuthResultValueFormatString, i));
                    stringBuilder.Append(i < spfAuthResultEntityBatch.Length - 1 ? "," : ";");

                    command.Parameters.AddWithValue($"a{i}", spfAuthResultEntityBatch[i].RecordId);
                    command.Parameters.AddWithValue($"b{i}", spfAuthResultEntityBatch[i].Domain);
                    command.Parameters.AddWithValue($"c{i}", spfAuthResultEntityBatch[i].Scope?.ToString());
                    command.Parameters.AddWithValue($"d{i}", spfAuthResultEntityBatch[i].Result?.ToString());
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        #endregion
    }
}