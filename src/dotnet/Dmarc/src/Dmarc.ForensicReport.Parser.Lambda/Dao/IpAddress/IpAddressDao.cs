using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.IpAddress
{
    public interface IIpAddressDao
    {
        Task<IpAddressEntity> Add(IpAddressEntity ipAddress, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class IpAddressDao : IIpAddressDao
    {
        public async Task<IpAddressEntity> Add(IpAddressEntity ipAddress, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand(IpAddressDaoResources.InsertIpAddress, connection, transaction);
            command.Parameters.AddWithValue("address", ipAddress.Ip);
            command.Parameters.AddWithValue("binary_address", ipAddress.BinaryIp);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            
            ipAddress.Id = command.LastInsertedId;
            return ipAddress;
        }
    }
}