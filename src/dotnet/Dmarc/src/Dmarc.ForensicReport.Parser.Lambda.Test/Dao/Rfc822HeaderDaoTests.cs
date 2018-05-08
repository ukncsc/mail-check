using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.EmailAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.IpAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822Header;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderField;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderTextValue;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class Rfc822HeaderDaoTests : DatabaseTestBase
    {
        private Rfc822HeaderDao _rfc822HeaderDao;
        private Rfc822HeaderFieldDao _rfc822HeaderFieldDao;
        private EmailAddressDao _emailAddressDao;
        private IpAddressDao _ipAddressDao;
        private Rfc822HeaderTextValueDao _rfc822HeaderTextValueDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _rfc822HeaderFieldDao = new Rfc822HeaderFieldDao();
            _emailAddressDao = new EmailAddressDao();
            _ipAddressDao = new IpAddressDao();
            _rfc822HeaderTextValueDao = new Rfc822HeaderTextValueDao();
            _rfc822HeaderDao = new Rfc822HeaderDao(_rfc822HeaderFieldDao, _emailAddressDao, _rfc822HeaderTextValueDao, _ipAddressDao);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddRfc822HeaderWithTextValueWhenDoesntExistCorrectlyAdded()
        {
            long headerSetId = GetHeaderSetId();

            Rfc822HeaderFieldEntity rfc822HeaderField = new Rfc822HeaderFieldEntity("AuthenticationResults", EntityRfc822HeaderValueType.Text);
            Rfc822HeaderEntity rfc822Header = new Rfc822HeaderEntity(rfc822HeaderField, 0, new Rfc822HeaderTextValueEntity("dmarc : fail"), null, null, null, null) { HeaderSetId = headerSetId };
            Rfc822HeaderEntity rfc822HeaderFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    rfc822HeaderFromDao = await _rfc822HeaderDao.Add(rfc822Header, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(rfc822HeaderFromDao.Date, Is.EqualTo(rfc822Header.Date));
            Assert.That(rfc822HeaderFromDao.Order, Is.EqualTo(rfc822Header.Order));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM rfc822_header_mapping"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("set_id"), Is.EqualTo(rfc822HeaderFromDao.HeaderSetId));
                    Assert.That(reader.GetInt64("field_id"), Is.EqualTo(rfc822HeaderFromDao.HeaderField.Id));
                    Assert.That(reader.GetInt16("order_in_set"), Is.EqualTo(rfc822HeaderFromDao.Order));
                    Assert.That(reader.GetInt64Nullable("text_value_id"), Is.EqualTo(rfc822HeaderFromDao.TextValue.Id));
                    Assert.That(reader.GetInt64Nullable("email_address_id"), Is.EqualTo(null));
                    Assert.That(reader.GetInt64Nullable("ip_address_id"), Is.EqualTo(null));
                    Assert.That(reader.GetInt64Nullable("hostname_id"), Is.EqualTo(null));
                    Assert.That(reader.GetDateTimeNullable("date"), Is.EqualTo(null));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddRfc822HeaderWithEmailAddressValueWhenDoesntExistCorrectlyAdded()
        {
            long headerSetId = GetHeaderSetId();

            Rfc822HeaderFieldEntity rfc822HeaderField = new Rfc822HeaderFieldEntity("To", EntityRfc822HeaderValueType.Email);
            Rfc822HeaderEntity rfc822Header = new Rfc822HeaderEntity(rfc822HeaderField, 0, null, new EmailAddressEntity("test@gov.uk"), null, null, null) {HeaderSetId = headerSetId};
            Rfc822HeaderEntity rfc822HeaderFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    rfc822HeaderFromDao = await _rfc822HeaderDao.Add(rfc822Header, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(rfc822HeaderFromDao.Date, Is.EqualTo(rfc822Header.Date));
            Assert.That(rfc822HeaderFromDao.Order, Is.EqualTo(rfc822Header.Order));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM rfc822_header_mapping"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("set_id"), Is.EqualTo(rfc822HeaderFromDao.HeaderSetId));
                    Assert.That(reader.GetInt64("field_id"), Is.EqualTo(rfc822HeaderFromDao.HeaderField.Id));
                    Assert.That(reader.GetInt16("order_in_set"), Is.EqualTo(rfc822HeaderFromDao.Order));
                    Assert.That(reader.GetInt64Nullable("text_value_id"), Is.EqualTo(null));
                    Assert.That(reader.GetInt64Nullable("email_address_id"), Is.EqualTo(rfc822HeaderFromDao.EmailAddress.Id));
                    Assert.That(reader.GetInt64Nullable("ip_address_id"), Is.EqualTo(null));
                    Assert.That(reader.GetInt64Nullable("hostname_id"), Is.EqualTo(null));
                    Assert.That(reader.GetDateTimeNullable("date"), Is.EqualTo(null));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddRfc822HeaderWithIpAddressValueWhenDoesntExistCorrectlyAdded()
        {
            long headerSetId = GetHeaderSetId();

            Rfc822HeaderFieldEntity rfc822HeaderField = new Rfc822HeaderFieldEntity("XOriginatingIp", EntityRfc822HeaderValueType.Ip);
            Rfc822HeaderEntity rfc822Header = new Rfc822HeaderEntity(rfc822HeaderField, 0, null, null, new IpAddressEntity("127.0.0.1", "0x7F000001"), null, null) { HeaderSetId = headerSetId };
            Rfc822HeaderEntity rfc822HeaderFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    rfc822HeaderFromDao = await _rfc822HeaderDao.Add(rfc822Header, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(rfc822HeaderFromDao.Date, Is.EqualTo(rfc822Header.Date));
            Assert.That(rfc822HeaderFromDao.Order, Is.EqualTo(rfc822Header.Order));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM rfc822_header_mapping"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("set_id"), Is.EqualTo(rfc822HeaderFromDao.HeaderSetId));
                    Assert.That(reader.GetInt64("field_id"), Is.EqualTo(rfc822HeaderFromDao.HeaderField.Id));
                    Assert.That(reader.GetInt16("order_in_set"), Is.EqualTo(rfc822HeaderFromDao.Order));
                    Assert.That(reader.GetInt64Nullable("text_value_id"), Is.EqualTo(null));
                    Assert.That(reader.GetInt64Nullable("email_address_id"), Is.EqualTo(null));
                    Assert.That(reader.GetInt64Nullable("ip_address_id"), Is.EqualTo(rfc822HeaderFromDao.IpAddress.Id));
                    Assert.That(reader.GetInt64Nullable("hostname_id"), Is.EqualTo(null));
                    Assert.That(reader.GetDateTimeNullable("date"), Is.EqualTo(null));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddTwiceThrows()
        {
            long headerSetId = GetHeaderSetId();

            Rfc822HeaderFieldEntity rfc822HeaderField = new Rfc822HeaderFieldEntity("AuthenticationResults", EntityRfc822HeaderValueType.Text);
            Rfc822HeaderEntity rfc822Header = new Rfc822HeaderEntity(rfc822HeaderField, 0, new Rfc822HeaderTextValueEntity("dmarc : fail"), null, null, null, null) { HeaderSetId = headerSetId };

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _rfc822HeaderDao.Add(rfc822Header, connection, transaction);
                    Assert.ThrowsAsync<MySqlException>(async () => await _rfc822HeaderDao.Add(rfc822Header, connection, transaction));
                    transaction.Commit();
                }
                connection.Close();
            }
        }


        private long GetHeaderSetId()
        {
            long ipAddressId = (long)(ulong)MySqlHelper.ExecuteScalar(ConnectionString, "INSERT INTO `ip_address` (`address`, `binary_address`, `subnet_id`) VALUES ('127.0.0.1', '0x7F000001', NULL); SELECT LAST_INSERT_ID();");
            long reportId = (long)(ulong)MySqlHelper.ExecuteScalar(ConnectionString, $"INSERT INTO `forensic_report` (`original_uri`, `feedback_type`, `user_agent`, `version`, `auth_failure`, `original_envelope_id`, `arrival_date`, " +
                                                                            $"`reporting_mta`, `source_ip_id`, `incidents`, `delivery_result`, `provider_message_id`, `message_id`, `dkim_domain`, `dkim_identity`, `dkim_selector`, " +
                                                                            $"`dkim_canonicalized_header`, `spf_dns`, `authentication_results`, `reported_domain`, `created_date`, `request_id`, `dkim_canonicalized_body`) VALUES " +
                                                                            $"('', 'NULL', NULL, NULL, NULL, NULL, NULL, NULL, {ipAddressId}, NULL, NULL, '', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2017-01-01', '', NULL); SELECT LAST_INSERT_ID();");

            long contentTypeId = (long)(ulong)MySqlHelper.ExecuteScalar(ConnectionString, "INSERT INTO `content_type` (`name`) VALUES(''); SELECT LAST_INSERT_ID();");


            return (long)(ulong)MySqlHelper.ExecuteScalar(ConnectionString, $"INSERT INTO `dmarc`.`rfc822_header_set` (`report_id`, `order`, `depth`, `content_type_id`) VALUES ({reportId}, 0, 0, {contentTypeId}); SELECT LAST_INSERT_ID();");
        }
    }
}
