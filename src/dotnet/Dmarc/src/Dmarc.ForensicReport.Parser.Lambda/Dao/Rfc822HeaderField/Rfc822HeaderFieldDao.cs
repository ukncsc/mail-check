using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderField
{
    public interface IRfc822HeaderFieldDao
    {
        Task<Rfc822HeaderFieldEntity> Add(Rfc822HeaderFieldEntity rfc822HeaderField, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class Rfc822HeaderFieldDao : IRfc822HeaderFieldDao
    {
        public async Task<Rfc822HeaderFieldEntity> Add(Rfc822HeaderFieldEntity rfc822HeaderField, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand(Rfc822HeaderFieldResources.InsertRfc822HeaderField, connection, transaction);
            command.Parameters.AddWithValue("name", rfc822HeaderField.Name);
            command.Parameters.AddWithValue("value_type", rfc822HeaderField.ValueType.GetDbName());

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            rfc822HeaderField.Id = command.LastInsertedId;

            return rfc822HeaderField;
        }
    }
}
