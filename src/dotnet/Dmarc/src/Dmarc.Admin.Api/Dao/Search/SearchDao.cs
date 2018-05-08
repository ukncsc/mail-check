using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Api.Identity.Domain;
using Dmarc.Common.Data;
using MySql.Data.MySqlClient;

namespace Dmarc.Admin.Api.Dao.Search
{
    public interface ISearchDao
    {
        Task<SearchResult> GetSearchResults(string search, int limit);
    }

    public class SearchDao : ISearchDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;

        public SearchDao(IConnectionInfoAsync connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task<SearchResult> GetSearchResults(string search, int limit)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = new MySqlCommand(SearchDaoResources.SelectUserGroupDomainByEmailNameName, connection);
                command.Parameters.AddWithValue("search", search);
                command.Parameters.AddWithValue("limit", limit);
               
                command.Prepare();

                List<Api.Domain.User> users = new List<Api.Domain.User>();
                List<Api.Domain.Group> groups = new List<Api.Domain.Group>();
                List<Api.Domain.Domain> domains = new List<Api.Domain.Domain>();
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string type = reader.GetString("type");
                        switch (type)
                        {
                            case "user":
                                users.Add(CreateUser(reader));
                                break;
                            case "group":
                                groups.Add(CreateGroup(reader));
                                break;
                            case "domain":
                                domains.Add(CreateDomain(reader));
                                break;
                            default:
                                throw new InvalidOperationException($"Unexpected entity type {type}");
                        }
                    }
                }
                connection.Close();

                return new SearchResult(
                    new Result<List<Api.Domain.Domain>>("Domain", domains), 
                    new Result<List<Api.Domain.User>>("User", users), 
                    new Result<List<Api.Domain.Group>>("Group", groups));
            }
        }

        private Api.Domain.Domain CreateDomain(DbDataReader reader)
        {
            return new Api.Domain.Domain(
                reader.GetInt32("id"),
                reader.GetString("name"));
        }

        private Api.Domain.Group CreateGroup(DbDataReader reader)
        {
            return new Api.Domain.Group(
                reader.GetInt32("id"),
                reader.GetString("name"));
        }

        private Api.Domain.User CreateUser(DbDataReader reader)
        {
            return new Api.Domain.User(
                reader.GetInt32("id"),
                reader.GetString("firstname"),
                reader.GetString("lastname"),
                reader.GetString("email"),
                reader.IsDbNull("global_admin")
                    ? RoleType.Standard
                    : reader.GetBoolean("global_admin") ? RoleType.Admin : RoleType.Standard);
        }
    }
}
