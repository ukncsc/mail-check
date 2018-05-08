using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.EmailAddress
{
    public interface IEmailAddressDao
    {
        Task<EmailAddressEntity> Add(EmailAddressEntity emailAddress, MySqlConnection connection,
            MySqlTransaction transaction);
    }
    public class EmailAddressDao : IEmailAddressDao
    {
        public async Task<EmailAddressEntity> Add(EmailAddressEntity emailAddress, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand(EmailAddressDaoResources.InsertEmailAddress, connection, transaction);
            command.Parameters.AddWithValue("address", emailAddress.EmailAddress);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            emailAddress.Id = command.LastInsertedId;

            return emailAddress;
        }
    }
}
