using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ContentType;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextContent;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicText
{
    public interface IForensicTextDao
    {
        Task<List<ForensicTextEntity>> Add(List<ForensicTextEntity> forensicTexts, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class ForensicTextDao : IForensicTextDao
    {
        private readonly IForensicTextContentDao _forensicTextContentDao;
        private readonly IContentTypeDao _contentTypeDao;

        public ForensicTextDao(IForensicTextContentDao forensicTextContentDao,
            IContentTypeDao contentTypeDao)
        {
            _forensicTextContentDao = forensicTextContentDao;
            _contentTypeDao = contentTypeDao;
        }

        public async Task<List<ForensicTextEntity>> Add(List<ForensicTextEntity> forensicTexts, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (!forensicTexts.Any())
            {
                return forensicTexts;
            }

            foreach (ForensicTextEntity forensicText in forensicTexts)
            {
                forensicText.ForensicTextContent = await _forensicTextContentDao.Add(forensicText.ForensicTextContent, connection, transaction);
                forensicText.ContentType = await _contentTypeDao.Add(forensicText.ContentType, connection, transaction);
            }

            MySqlCommand command = new MySqlCommand(connection, transaction);

            StringBuilder stringBuilder = new StringBuilder(ForensicTextDaoResources.InsertForensicText);

            for (int i = 0; i < forensicTexts.Count; i++)
            {
                stringBuilder.Append(string.Format(
                    ForensicTextDaoResources.InsertForensicTextValueFormatString, i));
                stringBuilder.Append(i < forensicTexts.Count - 1 ? "," : ";");

                command.Parameters.AddWithValue($"a{i}", forensicTexts[i].ReportId);
                command.Parameters.AddWithValue($"b{i}", forensicTexts[i].ForensicTextContent.Id);
                command.Parameters.AddWithValue($"c{i}", forensicTexts[i].Order);
                command.Parameters.AddWithValue($"d{i}", forensicTexts[i].Depth);
                command.Parameters.AddWithValue($"e{i}", forensicTexts[i].ContentType.Id);                
            }

            command.CommandText = stringBuilder.ToString();

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            return forensicTexts;
        }
    }
}