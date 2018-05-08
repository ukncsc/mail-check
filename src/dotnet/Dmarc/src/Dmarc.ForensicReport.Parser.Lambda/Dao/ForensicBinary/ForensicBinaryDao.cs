using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ContentType;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinaryContent;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinary
{
    public interface IForensicBinaryDao
    {
        Task<List<ForensicBinaryEntity>> Add(List<ForensicBinaryEntity> forensicBinaries, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class ForensicBinaryDao : IForensicBinaryDao
    {
        private readonly IForensicBinaryContentDao _forensicBinaryContentDao;
        private readonly IContentTypeDao _contentTypeDao;

        public ForensicBinaryDao(IForensicBinaryContentDao forensicBinaryContentDao,
            IContentTypeDao contentTypeDao)
        {
            _forensicBinaryContentDao = forensicBinaryContentDao;
            _contentTypeDao = contentTypeDao;
        }

        public async Task<List<ForensicBinaryEntity>> Add(List<ForensicBinaryEntity> forensicBinaries, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (!forensicBinaries.Any())
            {
                return forensicBinaries;
            }

            foreach (ForensicBinaryEntity forensicBinary in forensicBinaries)
            {
                forensicBinary.ForensicBinaryContent = await _forensicBinaryContentDao.Add(forensicBinary.ForensicBinaryContent, connection, transaction);
                forensicBinary.ContentType = await _contentTypeDao.Add(forensicBinary.ContentType, connection, transaction);
            }

            MySqlCommand command = new MySqlCommand(connection, transaction);

            StringBuilder stringBuilder = new StringBuilder(ForensicBinaryDaoResources.InsertForensicBinary);

            for (int i = 0; i < forensicBinaries.Count; i++)
            {
                stringBuilder.Append(string.Format(
                    ForensicBinaryDaoResources.InsertForensicBinaryValueFormatString, i));
                stringBuilder.Append(i < forensicBinaries.Count - 1 ? "," : ";");

                command.Parameters.AddWithValue($"a{i}", forensicBinaries[i].ReportId);
                command.Parameters.AddWithValue($"b{i}", forensicBinaries[i].ForensicBinaryContent.Id);
                command.Parameters.AddWithValue($"c{i}", forensicBinaries[i].Filename);
                command.Parameters.AddWithValue($"d{i}", forensicBinaries[i].Extension);
                command.Parameters.AddWithValue($"e{i}", forensicBinaries[i].Disposition?.GetDbName());
                command.Parameters.AddWithValue($"f{i}", forensicBinaries[i].ContentType.Id);
                command.Parameters.AddWithValue($"g{i}", forensicBinaries[i].Order);
                command.Parameters.AddWithValue($"h{i}", forensicBinaries[i].Depth);
            }

            command.CommandText = stringBuilder.ToString();

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            return forensicBinaries;
        }
    }
}
