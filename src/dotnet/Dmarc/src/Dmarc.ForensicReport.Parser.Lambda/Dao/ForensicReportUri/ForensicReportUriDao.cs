using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicUri;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicReportUri
{
    public interface IForensicReportUriDao
    {
        Task<List<ForensicReportUriEntity>> Add(List<ForensicReportUriEntity> forensicReportUris, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class ForensicReportUriDao : IForensicReportUriDao
    {
        private readonly IForensicUriDao _forensicUriDao;

        public ForensicReportUriDao(IForensicUriDao forensicUriDao)
        {
            _forensicUriDao = forensicUriDao;
        }

        public async Task<List<ForensicReportUriEntity>> Add(List<ForensicReportUriEntity> forensicReportUris, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (!forensicReportUris.Any())
            {
                return forensicReportUris;
            }

            foreach (ForensicReportUriEntity forensicReportUri in forensicReportUris)
            {
                forensicReportUri.ForensicUri = await _forensicUriDao.Add(forensicReportUri.ForensicUri, connection, transaction);
            }

            MySqlCommand command = new MySqlCommand(connection, transaction);

            StringBuilder stringBuilder = new StringBuilder(ForensicReportUriDaoResources.InsertForensicReportUri);

            for (int i = 0; i < forensicReportUris.Count; i++)
            {
                stringBuilder.Append(string.Format(ForensicReportUriDaoResources.InsertForensicReportUriValueFormatString, i));
                stringBuilder.Append(i < forensicReportUris.Count - 1 ? "," : ";");

                command.Parameters.AddWithValue($"a{i}", forensicReportUris[i].ReportId);
                command.Parameters.AddWithValue($"b{i}", forensicReportUris[i].ForensicUri.Id);
            }

            command.CommandText = stringBuilder.ToString();

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            return forensicReportUris;
        }
    }
}