using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Data;
using MySql.Data.MySqlClient;

namespace Dmarc.Admin.Api.Dao.Domain
{
    public interface IDomainDao
    {
        Task<Api.Domain.Domain> GetDomainById(int id);
        Task<List<Api.Domain.Domain>> GetDomainsByUserId(int userId, string search, int page, int pageSize);
        Task<List<Api.Domain.Domain>> GetDomainsByGroupId(int groupId, string search, int page, int pageSize);
        Task<List<Api.Domain.Domain>> GetDomainsByName(string search, int limit, List<int> includedIds);
        Task<Api.Domain.Domain> CreateDomain(string name, int createdBy);
    }

    public class DomainDao : IDomainDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;

        public DomainDao(IConnectionInfoAsync connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task<Api.Domain.Domain> GetDomainById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(DomainDaoResources.SelectDomainById, connection);
                command.Parameters.AddWithValue("id", id);

                command.Prepare();

                Api.Domain.Domain domain = null;
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        domain = new Api.Domain.Domain(
                            reader.GetInt32("id"),
                            reader.GetString("name"));
                    }
                }
                connection.Close();

                return domain;
            }
        }

        public async Task<List<Api.Domain.Domain>> GetDomainsByUserId(int userId, string search, int page, int pageSize)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(DomainDaoResources.SelectDomainsByUserId, connection);
                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("search", search);
                command.Parameters.AddWithValue("offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("pageSize", pageSize);

                command.Prepare();

                List<Api.Domain.Domain> domains = new List<Api.Domain.Domain>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        domains.Add(new Api.Domain.Domain(
                            reader.GetInt32("id"),
                            reader.GetString("name")));
                    }
                }
                connection.Close();

                return domains;
            }
        }

        public async Task<List<Api.Domain.Domain>> GetDomainsByGroupId(int groupId, string search, int page, int pageSize)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(DomainDaoResources.SelectDomainsByGroupId, connection);
                command.Parameters.AddWithValue("groupId", groupId);
                command.Parameters.AddWithValue("search", search);
                command.Parameters.AddWithValue("offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("pageSize", pageSize);

                command.Prepare();

                List<Api.Domain.Domain> domains = new List<Api.Domain.Domain>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        domains.Add(new Api.Domain.Domain(
                            reader.GetInt32("id"),
                            reader.GetString("name")));
                    }
                }
                connection.Close();

                return domains;
            }
        }

        public async Task<List<Api.Domain.Domain>> GetDomainsByName(string search, int limit, List<int> includedIds)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                string includedIdsString = includedIds.Any()
                    ? string.Join(",", Enumerable.Range(0, includedIds.Count).Select((_, i) => $"@a{i}"))
                    : "-1";

                string queryString = string.Format(DomainDaoResources.SelectDomainsByName, includedIdsString);

                MySqlCommand command = new MySqlCommand(queryString, connection);
                command.Parameters.AddWithValue("search", search);
                command.Parameters.AddWithValue("limit", limit);

                for (int i = 0; i < includedIds.Count; i++)
                {
                    command.Parameters.AddWithValue($"a{i}", includedIds[i]);
                }

                command.Prepare();

                List<Api.Domain.Domain> domains = new List<Api.Domain.Domain>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        domains.Add(new Api.Domain.Domain(
                            reader.GetInt32("id"),
                            reader.GetString("name")));
                    }
                }
                connection.Close();

                return domains;
            }
        }

        public async Task<Api.Domain.Domain> CreateDomain(string name, int createdBy)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                MySqlCommand command = new MySqlCommand(DomainDaoResources.InsertDomain, connection);
                command.Parameters.AddWithValue("name", name);
                command.Parameters.AddWithValue("createdBy", createdBy);
                command.Prepare();

                await command.ExecuteNonQueryAsync();

                Api.Domain.Domain newDomain = new Api.Domain.Domain(
                    (int)command.LastInsertedId,
                    name);

                connection.Close();

                return newDomain;
            }
        }
    }
}
