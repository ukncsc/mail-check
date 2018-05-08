using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.DnsRecord.Evaluator.Dmarc.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Dao
{
    public interface IDmarcConfigReadModelDao
    {
        Task InsertOrUpdate(List<DmarcConfigReadModelEntity> readModels);
    }

    public class DmarcConfigReadModelDao : IDmarcConfigReadModelDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger _log;

        public DmarcConfigReadModelDao(IConnectionInfoAsync connectionInfo, ILogger log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public async Task InsertOrUpdate(List<DmarcConfigReadModelEntity> readModels)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string connectionstring = await _connectionInfo.GetConnectionStringAsync();
            using (MySqlConnection connection = new MySqlConnection(connectionstring))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    MySqlCommand command = new MySqlCommand(connection, transaction);

                    StringBuilder stringBuilder = new StringBuilder(DmarcReadModelDaoResources.InsertOrUpdateRecord);

                    for (int i = 0; i < readModels.Count; i++)
                    {
                        stringBuilder.Append(string.Format(DmarcReadModelDaoResources.InsertOrUpdateValueFormatString, i));
                        stringBuilder.Append(i < readModels.Count - 1 ? "," : string.Empty);

                        command.Parameters.AddWithValue($"a{i}", readModels[i].DomainId);
                        command.Parameters.AddWithValue($"b{i}", readModels[i].ErrorCount);
                        command.Parameters.AddWithValue($"c{i}", readModels[i].MaxErrorSeverity?.ToString().ToLower());
                        command.Parameters.AddWithValue($"d{i}", readModels[i].ReadModel);
                    }

                    stringBuilder.Append(DmarcReadModelDaoResources.OnDuplicateKey);

                    command.CommandText = stringBuilder.ToString();
                    command.Prepare();

                    await command.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
                    connection.Close();
                }
            }
            _log.Debug($"Inserting {readModels.Count} DMARC record read models took {stopwatch.Elapsed}");
            stopwatch.Stop();
        }
    }
}
