using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.EmailAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.OriginalMailFrom
{
    public interface IOriginalMailFromDao
    {
        Task<List<EmailAddressReportEntity>> Add(List<EmailAddressReportEntity> originalMailFromEntities, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class OriginalMailFromDao : IOriginalMailFromDao
    {
        private readonly IEmailAddressDao _emailAddressDao;

        public OriginalMailFromDao(IEmailAddressDao emailAddressDao)
        {
            _emailAddressDao = emailAddressDao;
        }

        public async Task<List<EmailAddressReportEntity>> Add(List<EmailAddressReportEntity> originalMailFromEntities, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (!originalMailFromEntities.Any())
            {
                return originalMailFromEntities;
            }

            foreach (EmailAddressReportEntity originalMailFromEntity in originalMailFromEntities)
            {
                originalMailFromEntity.EmailAddressEntity = await _emailAddressDao.Add(originalMailFromEntity.EmailAddressEntity, connection, transaction);
            }

            MySqlCommand command = new MySqlCommand(connection, transaction);

            StringBuilder stringBuilder = new StringBuilder(OriginalMailFromDaoResources.InsertOriginalMailFrom);

            for (int i = 0; i < originalMailFromEntities.Count; i++)
            {
                stringBuilder.Append(string.Format(OriginalMailFromDaoResources.InsertOriginalMailFromValueFormatString, i));
                stringBuilder.Append(i < originalMailFromEntities.Count - 1 ? "," : ";");

                command.Parameters.AddWithValue($"a{i}", originalMailFromEntities[i].ReportId);
                command.Parameters.AddWithValue($"b{i}", originalMailFromEntities[i].EmailAddressEntity.Id);
            }

            command.CommandText = stringBuilder.ToString();

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            return originalMailFromEntities;
        }
    }
}
