using System.Threading.Tasks;
using Dmarc.Common.Data;
using MySql.Data.MySqlClient;

namespace Dmarc.Common.Api.Handlers
{
    public interface IPersistanceConnectionTester
    {
        Task TestConnection();
    }

    public class DatabaseConnectionTester : IPersistanceConnectionTester
    {
        private readonly IConnectionInfo _connectionInfo;

        public DatabaseConnectionTester(IConnectionInfo connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task TestConnection()
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionInfo.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                connection.Close();
            }
        }
    }
}