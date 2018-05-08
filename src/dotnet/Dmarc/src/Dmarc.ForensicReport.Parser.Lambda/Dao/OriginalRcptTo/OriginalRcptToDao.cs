using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.EmailAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.OriginalRcptTo
{
    public interface IOrginalRcptToDao
    {
        Task<List<EmailAddressReportEntity>> Add(List<EmailAddressReportEntity> originalRcptToEntities, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class OriginalRcptToDao : IOrginalRcptToDao
    {
        private readonly IEmailAddressDao _emailAddressDao;

        public OriginalRcptToDao(IEmailAddressDao  emailAddressDao)
        {
            _emailAddressDao = emailAddressDao;
        }
        
        public async Task<List<EmailAddressReportEntity>> Add(List<EmailAddressReportEntity> originalRcptToEntities, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (!originalRcptToEntities.Any())
            {
                return originalRcptToEntities;
            }


            foreach (EmailAddressReportEntity originalRcptToEntity in originalRcptToEntities)
            {
                originalRcptToEntity.EmailAddressEntity = await _emailAddressDao.Add(originalRcptToEntity.EmailAddressEntity, connection, transaction);
            }
            
            MySqlCommand command = new MySqlCommand(connection, transaction);

            StringBuilder stringBuilder = new StringBuilder(OriginalRcptToDaoResources.InsertOriginalRcptTo);

            for (int i = 0; i < originalRcptToEntities.Count; i++)
            {
                stringBuilder.Append(string.Format(OriginalRcptToDaoResources.InsertOriginalRcptToValueFormatString, i));
                stringBuilder.Append(i < originalRcptToEntities.Count - 1 ? "," : ";");

                command.Parameters.AddWithValue($"a{i}", originalRcptToEntities[i].ReportId);
                command.Parameters.AddWithValue($"b{i}", originalRcptToEntities[i].EmailAddressEntity.Id);
            }

            command.CommandText = stringBuilder.ToString();

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            return originalRcptToEntities;
        }
    }
}
