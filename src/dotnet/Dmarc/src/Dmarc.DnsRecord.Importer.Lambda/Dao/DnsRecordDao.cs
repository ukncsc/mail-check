using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Config;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.DnsRecord.Importer.Lambda.Dao
{
    public abstract class DnsRecordDao : IDnsRecordDao
    {
        private readonly IConnectionInfoAsync _connectionInfoAsync;
        private readonly IRecordImporterConfig _recordImporterConfig;
        private readonly ILogger _log;
        private readonly string _selectDomainsWithRecords;
        private readonly string _insertRecord;
        private readonly string _insertRecordValueFormatString;
        private readonly string _insertRecordOnDuplicateKey;

        protected DnsRecordDao(IConnectionInfoAsync connectionInfoAsync,
            IRecordImporterConfig recordImporterConfig,
            ILogger log,
            string selectDomainsWithRecords,
            string insertRecord,
            string insertRecordValueFormatString,
            string insertRecordOnDuplicateKey)
        {
            _connectionInfoAsync = connectionInfoAsync;
            _recordImporterConfig = recordImporterConfig;
            _log = log;
            _selectDomainsWithRecords = selectDomainsWithRecords;
            _insertRecord = insertRecord;
            _insertRecordValueFormatString = insertRecordValueFormatString;
            _insertRecordOnDuplicateKey = insertRecordOnDuplicateKey;
        }

        protected abstract Tuple<DomainEntity, RecordEntity> CreateRecordEntity(DbDataReader reader);

        protected abstract void AddCommandParmeters(MySqlCommand command, RecordEntity record, int index);

        public async Task<Dictionary<DomainEntity, List<RecordEntity>>> GetRecordsForUpdate()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Dictionary<DomainEntity, List<RecordEntity>> records = new Dictionary<DomainEntity, List<RecordEntity>>();

            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                _log.Debug($"Got connection!?");
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = new MySqlCommand(_selectDomainsWithRecords, connection);

                command.Parameters.AddWithValue("refreshIntervalSeconds", _recordImporterConfig.RefreshIntervalSeconds);
                command.Parameters.AddWithValue("failureRefreshIntervalSeconds", _recordImporterConfig.FailureRefreshIntervalSeconds);
                command.Parameters.AddWithValue("limit", _recordImporterConfig.DnsRecordLimit);

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Tuple<DomainEntity, RecordEntity> tuple = CreateRecordEntity(reader);

                        List<RecordEntity> values;
                        if (!records.TryGetValue(tuple.Item1, out values))
                        {
                            values = new List<RecordEntity>();
                        }

                        if (tuple.Item2 != null)
                        {
                            values.Add(tuple.Item2);
                        }

                        records[tuple.Item1] = values;
                    }
                }
                connection.Close();
            }
            stopwatch.Stop();

            _log.Debug($"Retrieving domains to refresh records for took {stopwatch.Elapsed}");

            return records;
        }

        public async Task InsertOrUpdateRecords(List<RecordEntity> records)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (records.Any())
            {
                using (MySqlConnection connection = new MySqlConnection(await _connectionInfoAsync.GetConnectionStringAsync()))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    StringBuilder stringBuilder = new StringBuilder(_insertRecord);
                    MySqlCommand command = new MySqlCommand { Connection = connection };

                    for (int i = 0; i < records.Count; i++)
                    {
                        stringBuilder.AppendFormat(_insertRecordValueFormatString, i);
                        stringBuilder.Append(i < records.Count - 1 ? "," : " ");

                        AddCommandParmeters(command, records[i], i);
                    }

                    stringBuilder.Append(_insertRecordOnDuplicateKey);

                    command.CommandText = stringBuilder.ToString();

                    await command.ExecuteNonQueryAsync();

                    connection.Close();
                }
            }

            stopwatch.Stop();

            _log.Debug($"Updating records took {stopwatch.Elapsed}");
        }
    }
}
