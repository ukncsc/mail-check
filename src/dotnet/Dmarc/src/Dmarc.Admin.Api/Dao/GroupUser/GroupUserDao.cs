using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using MySql.Data.MySqlClient;

namespace Dmarc.Admin.Api.Dao.GroupUser
{
    public interface IGroupUserDao
    {
        Task AddGroupUsers(List<Tuple<int, int>> groupUsers);
        Task DeleteGroupUsers(List<Tuple<int, int>> groupUsers);
    }

    public class GroupUserDao : IGroupUserDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;

        public GroupUserDao(IConnectionInfoAsync connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task AddGroupUsers(List<Tuple<int, int>> groupUsers)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                StringBuilder stringBuilder = new StringBuilder(GroupUserDaoResources.InsertGroupUser);
                MySqlCommand command = new MySqlCommand(string.Empty, connection);
                for (int i = 0; i < groupUsers.Count; i++)
                {
                    stringBuilder.Append(string.Format(GroupUserDaoResources.InsertGroupUserValueFormatString, i));
                    stringBuilder.Append(i < groupUsers.Count - 1 ? "," : ";");

                    command.Parameters.AddWithValue($"a{i}", groupUsers[i].Item1);
                    command.Parameters.AddWithValue($"b{i}", groupUsers[i].Item2);
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync();

                connection.Close();
            }
        }

        public async Task DeleteGroupUsers(List<Tuple<int, int>> groupUsers)
        {
            using (MySqlConnection connection = new MySqlConnection(await _connectionInfo.GetConnectionStringAsync()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                StringBuilder stringBuilder = new StringBuilder(GroupUserDaoResources.DeleteGroupUser);
                MySqlCommand command = new MySqlCommand(string.Empty, connection);
                for (int i = 0; i < groupUsers.Count; i++)
                {
                    stringBuilder.Append(string.Format(GroupUserDaoResources.DeleteGroupUserValueFormatString, i));
                    stringBuilder.Append(i < groupUsers.Count - 1 ? "," : ");");

                    command.Parameters.AddWithValue($"a{i}", groupUsers[i].Item1);
                    command.Parameters.AddWithValue($"b{i}", groupUsers[i].Item2);
                }

                command.CommandText = stringBuilder.ToString();

                await command.ExecuteNonQueryAsync();

                connection.Close();
            }
        }
    }
}
