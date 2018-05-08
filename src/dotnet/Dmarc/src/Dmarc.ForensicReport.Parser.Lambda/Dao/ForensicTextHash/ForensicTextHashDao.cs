using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextHash
{
    public interface IForensicTextHashDao
    {
        Task<HashEntity> Add(HashEntity forensicTextHash, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class ForensicTextHashDao : IForensicTextHashDao
    {
        public async Task<HashEntity> Add(HashEntity forensicTextHash, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand(ForensicTextHashDaoResources.InsertForensicTextHash, connection, transaction);
            command.Parameters.AddWithValue("text_id", forensicTextHash.ContentId);
            command.Parameters.AddWithValue("type", forensicTextHash.Type.GetDbName());
            command.Parameters.AddWithValue("hash", forensicTextHash.Hash);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            return forensicTextHash;
        }
    }
}