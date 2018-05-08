using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.DomainStatus.Api.Domain;
using MySql.Data.MySqlClient;

namespace Dmarc.DomainStatus.Api.Dao.Permission
{
    public interface IPermissionDao
    {
        Task<DomainPermissions> GetPermissions(int userId, int domainId);
    }

    public class PermissionDao : IPermissionDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;

        public PermissionDao(IConnectionInfoAsync connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task<DomainPermissions> GetPermissions(int userId, int domainId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                MySqlCommand command = new MySqlCommand(PermissionDaoResource.SelectPermissionByDomain, connection);

                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("domainId", domainId);

                command.Prepare();

                await connection.OpenAsync().ConfigureAwait(false);

                DbDataReader reader = await command.ExecuteReaderAsync();

                bool aggregatePermission = false;
                bool domainPermission = false; 
                while (await reader.ReadAsync())
                {
                    aggregatePermission = reader.GetBoolean("aggregate_permission");
                    domainPermission = reader.GetBoolean("domain_permission");
                }

                DomainPermissions domainPermissions = new DomainPermissions(domainId, aggregatePermission, domainPermission);

                connection.Close();
                return domainPermissions;
            }
        }
    }
}
