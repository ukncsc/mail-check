using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicUri;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextContentUri
{
    public interface IForensicTextContentUriDao
    {
        Task<List<ForensicTextContentUriEntity>> Add(List<ForensicTextContentUriEntity> forensicTextContentUris, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class ForensicTextContentUriDao : IForensicTextContentUriDao
    {
        private readonly IForensicUriDao _forensicUriDao;

        public ForensicTextContentUriDao(IForensicUriDao forensicUriDao)
        {
            _forensicUriDao = forensicUriDao;
        }

        public async Task<List<ForensicTextContentUriEntity>> Add(List<ForensicTextContentUriEntity> forensicTextContentUris, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (!forensicTextContentUris.Any())
            {
                return forensicTextContentUris;
            }

            foreach (ForensicTextContentUriEntity forensicTextContentUri in forensicTextContentUris)
            {
                forensicTextContentUri.ForensicUri = await _forensicUriDao.Add(forensicTextContentUri.ForensicUri, connection, transaction);
            }

            MySqlCommand command = new MySqlCommand(connection, transaction);

            StringBuilder stringBuilder = new StringBuilder(ForensicTextContentUriDaoResources.InsertForensicTextContentUri);

            for (int i = 0; i < forensicTextContentUris.Count; i++)
            {
                stringBuilder.Append(string.Format(ForensicTextContentUriDaoResources.InsertForensicTextContentUriValueFormatString, i));
                stringBuilder.Append(i < forensicTextContentUris.Count - 1 ? "," : ";");

                command.Parameters.AddWithValue($"a{i}", forensicTextContentUris[i].ForensicTextContentId);
                command.Parameters.AddWithValue($"b{i}", forensicTextContentUris[i].ForensicUri.Id);
            }

            command.CommandText = stringBuilder.ToString();

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            return forensicTextContentUris;
        }
    }
}
