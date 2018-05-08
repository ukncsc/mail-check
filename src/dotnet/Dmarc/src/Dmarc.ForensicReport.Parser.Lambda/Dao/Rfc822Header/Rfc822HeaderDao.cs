using System;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.EmailAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.IpAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderField;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderTextValue;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822Header
{
    public interface IRfc822HeaderDao
    {
        Task<Rfc822HeaderEntity> Add(Rfc822HeaderEntity rfc822HeaderEntity, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class Rfc822HeaderDao : IRfc822HeaderDao
    {
        private readonly IEmailAddressDao _emailAddressDao;
        private readonly IRfc822HeaderTextValueDao _rfc822HeaderTextValueDao;
        private readonly IIpAddressDao _ipAddressDao;
        private readonly IRfc822HeaderFieldDao _rfc822HeaderFieldDao;

        public Rfc822HeaderDao(IRfc822HeaderFieldDao rfc822HeaderFieldDao, 
            IEmailAddressDao emailAddressDao,
            IRfc822HeaderTextValueDao rfc822HeaderTextValueDao,
            IIpAddressDao ipAddressDao)
        {
            _emailAddressDao = emailAddressDao;
            _rfc822HeaderTextValueDao = rfc822HeaderTextValueDao;
            _ipAddressDao = ipAddressDao;
            _rfc822HeaderFieldDao = rfc822HeaderFieldDao;
        }

        public async Task<Rfc822HeaderEntity> Add(Rfc822HeaderEntity rfc822HeaderEntity, MySqlConnection connection, MySqlTransaction transaction)
        {
            rfc822HeaderEntity.HeaderField = await _rfc822HeaderFieldDao.Add(rfc822HeaderEntity.HeaderField, connection, transaction);

            string commandText;
            MySqlCommand command = new MySqlCommand(connection, transaction);
            switch (rfc822HeaderEntity.HeaderField.ValueType)
            {
                case EntityRfc822HeaderValueType.Text:
                    rfc822HeaderEntity.TextValue = await _rfc822HeaderTextValueDao.Add(rfc822HeaderEntity.TextValue, connection, transaction);
                    commandText = Rfc822HeaderDaoResources.InsertRfc822TextHeader;
                    command.Parameters.AddWithValue("text_value_id", rfc822HeaderEntity.TextValue.Id);
                    break;
                case EntityRfc822HeaderValueType.Email:
                    rfc822HeaderEntity.EmailAddress = await _emailAddressDao.Add(rfc822HeaderEntity.EmailAddress, connection, transaction);
                    commandText = Rfc822HeaderDaoResources.InsertRfc822EmailAddressHeader;
                    command.Parameters.AddWithValue("email_address_id", rfc822HeaderEntity.EmailAddress.Id);

                    break;
                case EntityRfc822HeaderValueType.Ip:
                    rfc822HeaderEntity.IpAddress = await _ipAddressDao.Add(rfc822HeaderEntity.IpAddress, connection, transaction);
                    commandText = Rfc822HeaderDaoResources.InsertRfc822IpAddressHeader;
                    command.Parameters.AddWithValue("ip_address_id", rfc822HeaderEntity.IpAddress.Id);
                    break;
                case EntityRfc822HeaderValueType.Date:
                    commandText = Rfc822HeaderDaoResources.InsertRfc822DateHeader;
                    command.Parameters.AddWithValue("date", rfc822HeaderEntity.Date);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unrecognised value {rfc822HeaderEntity.HeaderField.ValueType} for header field type.");
            }

            command.CommandText = commandText;
            
            command.Parameters.AddWithValue("field_id", rfc822HeaderEntity.HeaderField.Id);
            command.Parameters.AddWithValue("order_in_set", rfc822HeaderEntity.Order);
            command.Parameters.AddWithValue("set_id", rfc822HeaderEntity.HeaderSetId);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            rfc822HeaderEntity.Id = command.LastInsertedId;

            return rfc822HeaderEntity;
        }
    }
}
