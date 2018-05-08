using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Api.Identity.Domain;
using Dmarc.Common.Data;
using MySql.Data.MySqlClient;

namespace Dmarc.Common.Api.Identity.Dao
{
    public interface IIdentityDao
    {
        Task<Domain.Identity> GetIdentityByEmail(string email);
        Task<Domain.Identity> CreateIdentity(IdentityForCreation identity);
    }

    public class IdentityDao : IIdentityDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;

        public IdentityDao(IConnectionInfoAsync connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task<Domain.Identity> GetIdentityByEmail(string email)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(IdentityDaoResources.SelectUserByEmail, connection);
                command.Parameters.AddWithValue("email", email);

                command.Prepare();

                Domain.Identity identity = null;
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        identity = new Domain.Identity(
                        reader.GetInt32("id"),
                        reader.GetString("firstname"),
                        reader.GetString("lastname"),
                        reader.GetString("email"),
                        reader.IsDbNull("global_admin") ? RoleType.Standard : reader.GetBoolean("global_admin") ? RoleType.Admin : RoleType.Standard);
                    }
                }
                connection.Close();

                return identity;
            }
        }

        public async Task<Domain.Identity> CreateIdentity(IdentityForCreation identity)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(IdentityDaoResources.InsertUser, connection);
                command.Parameters.AddWithValue("firstname", identity.FirstName);
                command.Parameters.AddWithValue("lastname", identity.LastName);
                command.Parameters.AddWithValue("email", identity.Email);
                command.Parameters.AddWithValue("global_admin", identity.RoleType == RoleType.Admin);

                command.Prepare();

                await command.ExecuteNonQueryAsync();

                Domain.Identity newIdentity = new Domain.Identity(
                    (int)command.LastInsertedId,
                    identity.FirstName,
                    identity.LastName,
                    identity.Email,
                    identity.RoleType);

                connection.Close();

                return newIdentity;
            }
        }
    }
}