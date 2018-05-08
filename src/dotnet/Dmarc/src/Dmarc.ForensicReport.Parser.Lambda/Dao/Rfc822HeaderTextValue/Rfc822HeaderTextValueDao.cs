using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderTextValue
{
    public interface IRfc822HeaderTextValueDao
    {
        Task<Rfc822HeaderTextValueEntity> Add(Rfc822HeaderTextValueEntity textValue, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class Rfc822HeaderTextValueDao : IRfc822HeaderTextValueDao
    {
        public async Task<Rfc822HeaderTextValueEntity> Add(Rfc822HeaderTextValueEntity textValue, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand(Rfc822HeaderTextValueDaoResources.InsertRfc822HeaderTextValue, connection, transaction);
            command.Parameters.AddWithValue("value", textValue.Value);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            textValue.Id = command.LastInsertedId;

            return textValue;
        }
    }
}
