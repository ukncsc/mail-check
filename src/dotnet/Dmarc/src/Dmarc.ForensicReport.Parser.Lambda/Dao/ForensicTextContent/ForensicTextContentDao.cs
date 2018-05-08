using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextContentUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextHash;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Utils;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextContent
{
    public interface IForensicTextContentDao
    {
        Task<ForensicTextContentEntity> Add(ForensicTextContentEntity forensicTextContent, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class ForensicTextContentDao : IForensicTextContentDao
    {
        private readonly IForensicTextHashDao _forensicTextHashDao;
        private readonly IForensicTextContentUriDao _forensicTextContentUriDao;

        public ForensicTextContentDao(IForensicTextHashDao forensicTextHashDao,
            IForensicTextContentUriDao forensicTextContentUriDao)
        {
            _forensicTextHashDao = forensicTextHashDao;
            _forensicTextContentUriDao = forensicTextContentUriDao;
        }

        public async Task<ForensicTextContentEntity> Add(ForensicTextContentEntity forensicTextContent,
            MySqlConnection connection, MySqlTransaction transaction)
        {
            HashEntity sha1Hash = forensicTextContent.Hashes.Single(_ => _.Type == EntityHashType.Sha1);

            long? textId = await GetExistingForensicTextId(sha1Hash, connection, transaction);

            if (!textId.HasValue)
            {
                MySqlCommand command = new MySqlCommand(ForensicTextContentDaoResources.InsertForensicTextContent, connection, transaction);
                command.Parameters.AddWithValue("body", forensicTextContent.Text);

                await command.ExecuteNonQueryAsync();

                forensicTextContent.Id = command.LastInsertedId;

                forensicTextContent.Hashes.ForEach(_ => _.ContentId = forensicTextContent.Id);
                forensicTextContent.Uris.ForEach(_ => _.ForensicTextContentId = forensicTextContent.Id);
                
                foreach (HashEntity hash in forensicTextContent.Hashes)
                {
                    await _forensicTextHashDao.Add(hash, connection, transaction);
                }
                
                await _forensicTextContentUriDao.Add(forensicTextContent.Uris, connection, transaction);
            }
            else
            {
                forensicTextContent.Id = textId.Value;
            }
           
            return forensicTextContent;
        }

        private async Task<long?> GetExistingForensicTextId(HashEntity hashEntity, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand(ForensicTextContentDaoResources.SelectForensicTextHash, connection, transaction);
            command.Parameters.AddWithValue("type", hashEntity.Type.GetDbName());
            command.Parameters.AddWithValue("hash", hashEntity.Hash);

            object textId = await command.ExecuteScalarAsync().ConfigureAwait(false);

            return textId == null ? null : (long?) (ulong)textId;
        }
    }
}