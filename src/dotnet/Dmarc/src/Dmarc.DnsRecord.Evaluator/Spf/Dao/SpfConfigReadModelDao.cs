using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.DnsRecord.Evaluator.Spf.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.DnsRecord.Evaluator.Spf.Dao
{
    public interface ISpfConfigReadModelDao
    {
        Task InsertOrUpdate(List<SpfConfigReadModelEntity> readModels);
    }

    public class SpfConfigReadModelDao : ISpfConfigReadModelDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger _log;

        public SpfConfigReadModelDao(IConnectionInfoAsync connectionInfo, ILogger log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public async Task InsertOrUpdate(List<SpfConfigReadModelEntity> readModels)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    MySqlCommand command = new MySqlCommand(connection, transaction);

                    StringBuilder stringBuilder = new StringBuilder(SpfReadModelDaoResources.InsertOrUpdateRecord);

                    for (int i = 0; i < readModels.Count; i++)
                    {
                        stringBuilder.Append(string.Format(SpfReadModelDaoResources.InsertOrUpdateValueFormatString, i));
                        stringBuilder.Append(i < readModels.Count - 1 ? "," : string.Empty);

                        command.Parameters.AddWithValue($"a{i}", readModels[i].DomainId);
                        command.Parameters.AddWithValue($"b{i}", readModels[i].ErrorCount);
                        command.Parameters.AddWithValue($"c{i}", readModels[i].MaxErrorSeverity?.ToString().ToLower());
                        command.Parameters.AddWithValue($"d{i}", readModels[i].ReadModel);
                    }

                    stringBuilder.Append(SpfReadModelDaoResources.OnDuplicateKey);

                    command.CommandText = stringBuilder.ToString();
                    command.Prepare();

                    await command.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
                    connection.Close();
                }
            }
            _log.Debug($"Inserting {readModels.Count} SPF record read models took {stopwatch.Elapsed}");
            stopwatch.Stop();
        }
    }
}
