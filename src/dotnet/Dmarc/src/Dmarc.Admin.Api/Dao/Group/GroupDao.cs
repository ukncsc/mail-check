using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Data;
using MySql.Data.MySqlClient;

namespace Dmarc.Admin.Api.Dao.Group
{
    public interface IGroupDao
    {
        Task<Api.Domain.Group> GetGroupById(int id);
        Task<List<Api.Domain.Group>> GetGroupsByUserId(int userId, string search, int page, int pageSize);
        Task<List<Api.Domain.Group>> GetGroupsByDomainId(int domainId, string search, int page, int pageSize);
        Task<List<Api.Domain.Group>> GetGroupsByName(string search, int limit, List<int> includedIds);
        Task<Api.Domain.Group> CreateGroup(GroupForCreation user);
    }

    public class GroupDao : IGroupDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;

        public GroupDao(IConnectionInfoAsync connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task<Api.Domain.Group> GetGroupById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(GroupDaoResources.SelectGroupById, connection);
                command.Parameters.AddWithValue("id", id);

                command.Prepare();

                Api.Domain.Group group = null;
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        group = new Api.Domain.Group(
                            reader.GetInt32("id"),
                            reader.GetString("name"));
                    }
                }
                connection.Close();

                return group;
            }
        }

        public async Task<List<Api.Domain.Group>> GetGroupsByUserId(int userId, string search, int page, int pageSize)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(GroupDaoResources.SelectGroupsByUserId, connection);
                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("search", search);
                command.Parameters.AddWithValue("offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("pageSize", pageSize);

                command.Prepare();

                List<Api.Domain.Group> groups = new List<Api.Domain.Group>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        groups.Add(new Api.Domain.Group(
                            reader.GetInt32("id"),
                            reader.GetString("name")));
                    }
                }
                connection.Close();

                return groups;
            }
        }

        public async Task<List<Api.Domain.Group>> GetGroupsByDomainId(int domainId, string search, int page, int pageSize)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(GroupDaoResources.SelectGroupsByDomainId, connection);
                command.Parameters.AddWithValue("domainId", domainId);
                command.Parameters.AddWithValue("search", search);
                command.Parameters.AddWithValue("offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("pageSize", pageSize);

                command.Prepare();

                List<Api.Domain.Group> groups = new List<Api.Domain.Group>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        groups.Add(new Api.Domain.Group(
                            reader.GetInt32("id"),
                            reader.GetString("name")));
                    }
                }
                connection.Close();

                return groups;
            }
        }

        public async Task<List<Api.Domain.Group>> GetGroupsByName(string search, int limit, List<int> includedIds)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                string includedIdsString = includedIds.Any()
                    ? string.Join(",", Enumerable.Range(0, includedIds.Count).Select((_, i) => $"@a{i}"))
                    : "-1";

                string queryString = string.Format(GroupDaoResources.SelectGroupsByName, includedIdsString);

                MySqlCommand command = new MySqlCommand(queryString, connection);
                command.Parameters.AddWithValue("search", search);
                command.Parameters.AddWithValue("limit", limit);

                for (int i = 0; i < includedIds.Count; i++)
                {
                    command.Parameters.AddWithValue($"a{i}", includedIds[i]);
                }

                command.Prepare();

                List<Api.Domain.Group> groups = new List<Api.Domain.Group>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        groups.Add(new Api.Domain.Group(
                            reader.GetInt32("id"),
                            reader.GetString("name")));
                    }
                }
                connection.Close();

                return groups;
            }
        }

        public async Task<Api.Domain.Group> CreateGroup(GroupForCreation user)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(GroupDaoResources.InsertGroup, connection);
                command.Parameters.AddWithValue("name", user.Name);

                command.Prepare();

                ulong groupId = (ulong)await command.ExecuteScalarAsync();

                Api.Domain.Group newGroup = new Api.Domain.Group(
                    (int) groupId,
                    user.Name);

                connection.Close();

                return newGroup;
            }
        }
    }
}
