using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinaryHash;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Utils;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinaryContent
{
    public interface IForensicBinaryContentDao
    {
        Task<ForensicBinaryContentEntity> Add(ForensicBinaryContentEntity forensicBinaryContent, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class ForensicBinaryContentDao : IForensicBinaryContentDao
    {
        private readonly IForensicBinaryHashDao _forensicBinaryHashDao;

        public ForensicBinaryContentDao(IForensicBinaryHashDao forensicBinaryHashDao)
        {
            _forensicBinaryHashDao = forensicBinaryHashDao;
        }

        public async Task<ForensicBinaryContentEntity> Add(ForensicBinaryContentEntity forensicBinaryContent, MySqlConnection connection, MySqlTransaction transaction)
        {
            HashEntity sha1Hash = forensicBinaryContent.Hashes.Single(_ => _.Type == EntityHashType.Sha1);

            MySqlCommand command = new MySqlCommand(ForensicBinaryContentDaoResources.InsertForensicBinaryContent, connection, transaction);
            command.Parameters.AddWithValue("attachment", forensicBinaryContent.Content);
            command.Parameters.AddWithValue("hash", sha1Hash.Hash);

            long forensicBinaryContentId = -1;
            int recordsAffected = -1;
            using (DbDataReader dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                recordsAffected = dataReader.RecordsAffected;
                while (dataReader.Read())
                {
                    forensicBinaryContentId = dataReader.GetInt64("binary_id");
                }
            }
            
            forensicBinaryContent.Id = forensicBinaryContentId;

            if (recordsAffected > 0)
            {
                forensicBinaryContent.Hashes.ForEach(_ => _.ContentId = forensicBinaryContentId);
                
                foreach (HashEntity hash in forensicBinaryContent.Hashes)
                {
                    await _forensicBinaryHashDao.Add(hash, connection, transaction);
                }
            }

            return forensicBinaryContent;
        }
    }
}