using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Api.Identity.Domain;
using Dmarc.Common.Data;
using MySql.Data.MySqlClient;

namespace Dmarc.Admin.Api.Dao.User
{
    public interface IUserDao
    {
        Task<Api.Domain.User> GetUserById(int id);
        Task<List<Api.Domain.User>> GetUsersByGroupId(int groupId, string search, int page, int pageSize);
        Task<List<Api.Domain.User>> GetUsersByDomainId(int domainId, string search, int page, int pageSize);
        Task<List<Api.Domain.User>> GetUsersByFirstNameLastNameEmail(string search, int limit, List<int> includedIds);
        Task<Api.Domain.User> CreateUser(UserForCreation user);
        Task<List<DomainPermission>> GetDomainPermissions(int userId);
    }

    public class UserDao : IUserDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;

        public UserDao(IConnectionInfoAsync connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task<Api.Domain.User> GetUserById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(UserDaoResources.SelectUserById, connection);
                command.Parameters.AddWithValue("id", id);

                command.Prepare();

                Api.Domain.User user = null;
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        user = new Api.Domain.User(
                        reader.GetInt32("id"),
                        reader.GetString("firstname"),
                        reader.GetString("lastname"),
                        reader.GetString("email"),
                        reader.IsDbNull("global_admin") ? RoleType.Standard : reader.GetBoolean("global_admin") ? RoleType.Admin : RoleType.Standard);
                    }
                }
                connection.Close();

                return user;
            }
        }

        public async Task<List<Api.Domain.User>> GetUsersByGroupId(int groupId, string search, int page, int pageSize)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(UserDaoResources.SelectUsersByGroupId, connection);
                command.Parameters.AddWithValue("groupId", groupId);
                command.Parameters.AddWithValue("search", search);
                command.Parameters.AddWithValue("offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("pageSize", pageSize);

                command.Prepare();

                List<Api.Domain.User> users = new List<Api.Domain.User>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new Api.Domain.User(
                        reader.GetInt32("id"),
                        reader.GetString("firstname"),
                        reader.GetString("lastname"),
                        reader.GetString("email"),
                        reader.IsDbNull("global_admin") ? RoleType.Standard : reader.GetBoolean("global_admin") ? RoleType.Admin : RoleType.Standard));
                    }
                }
                connection.Close();

                return users;
            }
        }

        public async Task<List<Api.Domain.User>> GetUsersByDomainId(int domainId, string search, int page, int pageSize)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(UserDaoResources.SelectUsersByDomainId, connection);
                command.Parameters.AddWithValue("domainId", domainId);
                command.Parameters.AddWithValue("search", search);
                command.Parameters.AddWithValue("offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("pageSize", pageSize);

                command.Prepare();

                List<Api.Domain.User> users = new List<Api.Domain.User>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new Api.Domain.User(
                        reader.GetInt32("id"),
                        reader.GetString("firstname"),
                        reader.GetString("lastname"),
                        reader.GetString("email"),
                        reader.IsDbNull("global_admin") ? RoleType.Standard : reader.GetBoolean("global_admin") ? RoleType.Admin : RoleType.Standard));
                    }
                }
                connection.Close();

                return users;
            }
        }

        public async Task<List<Api.Domain.User>> GetUsersByFirstNameLastNameEmail(string search, int limit, List<int> includedIds)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                string includedIdsString = includedIds.Any()
                   ? string.Join(",", Enumerable.Range(0, includedIds.Count).Select((_, i) => $"@a{i}"))
                   : "-1";

                string queryString = string.Format(UserDaoResources.SelectUsersByEmailFirstNameLastName, includedIdsString);

                MySqlCommand command = new MySqlCommand(queryString, connection);
                command.Parameters.AddWithValue("search", search);
                command.Parameters.AddWithValue("limit", limit);

                for (int i = 0; i < includedIds.Count; i++)
                {
                    command.Parameters.AddWithValue($"a{i}", includedIds[i]);
                }

                command.Prepare();

                List<Api.Domain.User> users = new List<Api.Domain.User>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new Api.Domain.User(
                        reader.GetInt32("id"),
                        reader.GetString("firstname"),
                        reader.GetString("lastname"),
                        reader.GetString("email"),
                        reader.IsDbNull("global_admin") ? RoleType.Standard : reader.GetBoolean("global_admin") ? RoleType.Admin : RoleType.Standard));
                    }
                }
                connection.Close();

                return users;
            }
        }

        public async Task<Api.Domain.User> CreateUser(UserForCreation user)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(UserDaoResources.InsertUser, connection);
                command.Parameters.AddWithValue("firstname", user.FirstName);
                command.Parameters.AddWithValue("lastname", user.LastName);
                command.Parameters.AddWithValue("email", user.Email);
                command.Parameters.AddWithValue("global_admin", user.IsAdmin);

                command.Prepare();

                ulong userId = (ulong)await command.ExecuteScalarAsync();

                Api.Domain.User newUser = new Api.Domain.User(
                    (int)userId, 
                    user.FirstName, 
                    user.LastName, 
                    user.Email, 
                    user.IsAdmin ? RoleType.Admin : RoleType.Standard);

                connection.Close();

                return newUser;
            }
        }

        public async Task<List<DomainPermission>> GetDomainPermissions(int userId)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(UserDaoResources.SelectDomainPermissions, connection);
                command.Parameters.AddWithValue("userId", userId);

                command.Prepare();

                DbDataReader reader = await command.ExecuteReaderAsync();

                List<DomainPermission> domainPermissions = new List<DomainPermission>();

                while (await reader.ReadAsync())
                {
                    DomainPermission domainPermission = new DomainPermission(
                        new Api.Domain.Domain(
                            reader.GetInt32("domain_id"), 
                            reader.GetString("domain_name")),
                        new List<Permission> {Permission.ViewAggregateData});

                    domainPermissions.Add(domainPermission);
                }
                connection.Close();

                return domainPermissions;
            }
        }
    }
}
