using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using MySql.Data.MySqlClient;

namespace Dmarc.DnsRecord.Evaluator.Seeding.Dao
{
    public interface IDnsRecordDao<T>
    {
        Task<List<T>> GetCurrentRecords();
    }

    public abstract class DnsRecordDao<T> : IDnsRecordDao<T>
    {
        private readonly IConnectionInfo _connectionInfo;
        private readonly string _selectCurrentRecords;

        protected DnsRecordDao(IConnectionInfo connectionInfo, 
            string selectCurrentRecords)
        {
            _connectionInfo = connectionInfo;
            _selectCurrentRecords = selectCurrentRecords;
        }

        public async Task<List<T>> GetCurrentRecords()
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(_selectCurrentRecords, connection);
                command.Prepare();

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    return  await CreateRecords(reader);
                }
            }
        }

        protected abstract Task<List<T>> CreateRecords(DbDataReader reader);
    }
}