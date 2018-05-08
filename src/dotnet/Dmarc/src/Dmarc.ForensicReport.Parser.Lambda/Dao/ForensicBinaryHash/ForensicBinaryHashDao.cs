using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinaryHash
{
    public interface IForensicBinaryHashDao
    {
        Task<HashEntity> Add(HashEntity hash, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class ForensicBinaryHashDao : IForensicBinaryHashDao
    {
        public async Task<HashEntity> Add(HashEntity hash, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand(ForensicBinaryHashDaoResources.InsertForensicBinaryHash, connection, transaction);
            command.Parameters.AddWithValue("binary_id", hash.ContentId);
            command.Parameters.AddWithValue("type", hash.Type.GetDbName());
            command.Parameters.AddWithValue("hash", hash.Hash);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            return hash;
        }
    }
}