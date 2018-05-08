using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using MySql.Data.MySqlClient;

namespace Dmarc.Admin.Api.Dao.GroupDomain
{
    public interface IGroupDomainDao
    {
        Task AddGroupDomains(List<Tuple<int, int>> groupDomains);
        Task DeleteGroupDomains(List<Tuple<int, int>> groupDomains);
    }

    public class GroupDomainDao : IGroupDomainDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;

        public GroupDomainDao(IConnectionInfoAsync connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task AddGroupDomains(List<Tuple<int, int>> groupDomains)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                StringBuilder stringBuilder = new StringBuilder(GroupDomainDaoResources.InsertGroupDomain);
                MySqlCommand command = new MySqlCommand(string.Empty, connection);
                for (int i = 0; i < groupDomains.Count; i++)
                {
                    stringBuilder.Append(string.Format(GroupDomainDaoResources.InsertGroupDomainValueFormatString, i));
                    stringBuilder.Append(i < groupDomains.Count - 1 ? "," : ";");

                    command.Parameters.AddWithValue($"a{i}", groupDomains[i].Item1);
                    command.Parameters.AddWithValue($"b{i}", groupDomains[i].Item2);
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync();

                connection.Close();
            }
        }

        public async Task DeleteGroupDomains(List<Tuple<int, int>> groupDomains)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                StringBuilder stringBuilder = new StringBuilder(GroupDomainDaoResources.DeleteGroupDomain);
                MySqlCommand command = new MySqlCommand(string.Empty, connection);
                for (int i = 0; i < groupDomains.Count; i++)
                {
                    stringBuilder.Append(string.Format(GroupDomainDaoResources.DeleteGroupDomainValueFormatString, i));
                    stringBuilder.Append(i < groupDomains.Count - 1 ? "," : ");");

                    command.Parameters.AddWithValue($"a{i}", groupDomains[i].Item1);
                    command.Parameters.AddWithValue($"b{i}", groupDomains[i].Item2);
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync();

                connection.Close();
            }
        }
    }
}
