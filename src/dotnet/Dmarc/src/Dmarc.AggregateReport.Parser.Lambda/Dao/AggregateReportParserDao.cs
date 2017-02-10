using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Parser.Lambda.Dao.Entities;
using Dmarc.Common.Data;
using Dmarc.Common.Linq;
using Dmarc.Common.Logging;
using MySql.Data.MySqlClient;

namespace Dmarc.AggregateReport.Parser.Lambda.Dao
{
    internal interface IAggregateReportDao
    {
        Task Add(Entities.AggregateReport aggregateReport);
    }

    internal class AggregateReportParserDao : IAggregateReportDao
    {
        private const int BatchSize = 10000;

        private readonly IConnectionInfo _connectionInfo;
        private readonly ILogger _log;

        public AggregateReportParserDao(IConnectionInfo connectionInfo, ILogger log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        #region Aggregate Report

        public async Task Add(Entities.AggregateReport aggregateReport)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    MySqlCommand command = new MySqlCommand(AggregateReportParserDaoResources.InsertAggregateReport,
                        connection, transaction);
                    command.Parameters.AddWithValue("request_id", aggregateReport.RequestId);
                    command.Parameters.AddWithValue("original_uri", aggregateReport.OrginalUri);
                    command.Parameters.AddWithValue("attachment_filename", aggregateReport.AttachmentFilename);
                    command.Parameters.AddWithValue("org_name", aggregateReport.OrgName);
                    command.Parameters.AddWithValue("email", aggregateReport.Email);
                    command.Parameters.AddWithValue("report_id", aggregateReport.ReportId);
                    command.Parameters.AddWithValue("extra_contact_info", aggregateReport.ExtraContactInfo);
                    command.Parameters.AddWithValue("begin_date", aggregateReport.BeginDate);
                    command.Parameters.AddWithValue("end_date", aggregateReport.EndDate);
                    command.Parameters.AddWithValue("domain", aggregateReport.Domain);
                    command.Parameters.AddWithValue("adkim", aggregateReport.Adkim?.ToString());
                    command.Parameters.AddWithValue("aspf", aggregateReport.Aspf?.ToString());
                    command.Parameters.AddWithValue("p", aggregateReport.P.ToString());
                    command.Parameters.AddWithValue("sp", aggregateReport.Sp?.ToString());
                    command.Parameters.AddWithValue("pct", aggregateReport.Pct);
                    command.Parameters.AddWithValue("created_date", DateTime.UtcNow);
                    int numberOfUpdates = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                    if (numberOfUpdates != 0) // number of updated == 0 -> duplicate report id.             
                    {
                        aggregateReport.Id = command.LastInsertedId;

                        foreach (var record in aggregateReport.Records)
                        {
                            record.AggregateReportId = aggregateReport.Id;
                        }

                        await AddRecords(aggregateReport.Records.ToArray(), connection, transaction).ConfigureAwait(false);
                    }
                    else
                    {
                        _log.Error($"Duplicate aggregate report, request id: {aggregateReport.RequestId}, original uri: {aggregateReport.OrginalUri}, report id: {aggregateReport.ReportId}");
                    }
                    await transaction.CommitAsync().ConfigureAwait(false);
                }
            }
            _log.Debug($"Persisting aggregate report took {stopwatch.Elapsed}");
            stopwatch.Stop();
        }

        #endregion Aggregate Report

        #region Record

        private async Task AddRecords(Record[] records, MySqlConnection connection, MySqlTransaction transaction)
        {
            foreach (var batch in records.Batch(BatchSize))
            {
                Record[] recordBatch = batch.ToArray();
                MySqlCommand command = new MySqlCommand(connection, transaction);

                StringBuilder stringBuilder = new StringBuilder(AggregateReportParserDaoResources.InsertRecord);
                for (int i = 0; i < recordBatch.Length; i++)
                {
                    stringBuilder.Append(string.Format(AggregateReportParserDaoResources.InsertRecordValueFormatString, i));
                    stringBuilder.Append(i < recordBatch.Length - 1 ? "," : ";");

                    command.Parameters.AddWithValue($"a{i}", recordBatch[i].AggregateReportId);
                    command.Parameters.AddWithValue($"b{i}", recordBatch[i].SourceIp);
                    command.Parameters.AddWithValue($"c{i}", recordBatch[i].Count);
                    command.Parameters.AddWithValue($"d{i}", recordBatch[i].Disposition?.ToString());
                    command.Parameters.AddWithValue($"e{i}", recordBatch[i].Dkim?.ToString());
                    command.Parameters.AddWithValue($"f{i}", recordBatch[i].Spf.ToString());
                    command.Parameters.AddWithValue($"g{i}", recordBatch[i].EnvelopeTo);
                    command.Parameters.AddWithValue($"h{i}", recordBatch[i].HeaderFrom);
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                long lastInsertId = command.LastInsertedId;
                foreach (Record record in recordBatch)
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

            await AddPolicyOverrideReasons(records.SelectMany(_ => _.Reason).ToArray(), connection, transaction).ConfigureAwait(false);
            await AddDkimAuthResults(records.SelectMany(_ => _.DkimAuthResults).ToArray(), connection, transaction).ConfigureAwait(false);
            await AddSpfAuthResults(records.SelectMany(_ => _.SpfAuthResults).ToArray(), connection, transaction).ConfigureAwait(false);
        }
        #endregion Record

        #region PolicyOverrideReason

        private async Task AddPolicyOverrideReasons(PolicyOverrideReason[] reasons, MySqlConnection connection,
            MySqlTransaction transaction)
        {
            foreach (var batch in reasons.Batch(BatchSize))
            {
                PolicyOverrideReason[] reasonsBatch = batch.ToArray();
                MySqlCommand command = new MySqlCommand(connection, transaction);
                command.Transaction = transaction;

                StringBuilder stringBuilder = new StringBuilder(AggregateReportParserDaoResources.InsertPolicyOverrideReason);

                for (int i = 0; i < reasonsBatch.Length; i++)
                {
                    stringBuilder.Append(
                        string.Format(AggregateReportParserDaoResources.InsertPolicyOverrideReasonValueFormatString, i));
                    stringBuilder.Append(i < reasonsBatch.Length - 1 ? "," : ";");

                    command.Parameters.AddWithValue($"a{i}", reasonsBatch[i].RecordId);
                    command.Parameters.AddWithValue($"b{i}", reasonsBatch[i].PolicyOverride?.ToString());
                    command.Parameters.AddWithValue($"c{i}", reasonsBatch[i].Comment);
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region DkimAuthResults

        private async Task AddDkimAuthResults(DkimAuthResult[] dkimAuthResults, MySqlConnection connection,
            MySqlTransaction transaction)
        {
            foreach (var batch in dkimAuthResults.Batch(BatchSize))
            {
                DkimAuthResult[] dkimAuthResultBatch = batch.ToArray();
                MySqlCommand command = new MySqlCommand(connection, transaction);
                command.Transaction = transaction;

                StringBuilder stringBuilder = new StringBuilder(AggregateReportParserDaoResources.InsertDkimAuthResult);

                for (int i = 0; i < dkimAuthResultBatch.Length; i++)
                {
                    stringBuilder.Append(string.Format(
                        AggregateReportParserDaoResources.InsertDkimAuthResultValueFormatString, i));
                    stringBuilder.Append(i < dkimAuthResultBatch.Length - 1 ? "," : ";");

                    command.Parameters.AddWithValue($"a{i}", dkimAuthResultBatch[i].RecordId);
                    command.Parameters.AddWithValue($"b{i}", dkimAuthResultBatch[i].Domain);
                    command.Parameters.AddWithValue($"c{i}", dkimAuthResultBatch[i].Result?.ToString());
                    command.Parameters.AddWithValue($"d{i}", dkimAuthResultBatch[i].HumanResult);
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region SpfAuthResults

        private async Task AddSpfAuthResults(SpfAuthResult[] spfAuthResults, MySqlConnection connection,
            MySqlTransaction transaction)
        {
            foreach (var batch in spfAuthResults.Batch(BatchSize))
            {
                SpfAuthResult[] spfAuthResultBatch = batch.ToArray();
                MySqlCommand command = new MySqlCommand(connection, transaction);
                command.Transaction = transaction;

                StringBuilder stringBuilder = new StringBuilder(AggregateReportParserDaoResources.InsertSpfAuthResult);

                for (int i = 0; i < spfAuthResultBatch.Length; i++)
                {
                    stringBuilder.Append(string.Format(
                        AggregateReportParserDaoResources.InsertSpfAuthResultValueFormatString, i));
                    stringBuilder.Append(i < spfAuthResultBatch.Length - 1 ? "," : ";");

                    command.Parameters.AddWithValue($"a{i}", spfAuthResultBatch[i].RecordId);
                    command.Parameters.AddWithValue($"b{i}", spfAuthResultBatch[i].Domain);
                    command.Parameters.AddWithValue($"c{i}", spfAuthResultBatch[i].Result?.ToString());
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        #endregion
    }
}