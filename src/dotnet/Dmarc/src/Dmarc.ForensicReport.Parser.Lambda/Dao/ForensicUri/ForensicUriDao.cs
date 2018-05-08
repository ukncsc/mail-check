using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicUri
{
    public interface IForensicUriDao
    {
        Task<ForensicUriEntity> Add(ForensicUriEntity uriEntity, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class ForensicUriDao : IForensicUriDao
    {
        public async Task<ForensicUriEntity> Add(ForensicUriEntity uriEntity, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand(ForensicUriDaoResources.InsertForensicUri, connection, transaction);
            command.Parameters.AddWithValue("uri", uriEntity.Uri);
            command.Parameters.AddWithValue("uri_hash_sha256", uriEntity.Sha256);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            uriEntity.Id = command.LastInsertedId;

            return uriEntity;
        }
    }
}