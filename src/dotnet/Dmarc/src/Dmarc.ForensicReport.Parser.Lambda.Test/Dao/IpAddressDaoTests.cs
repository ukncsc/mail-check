using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.IpAddress;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class IpAddressDaoTests : DatabaseTestBase
    {
        private IpAddressDao _ipAddressDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _ipAddressDao = new IpAddressDao();
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddIpAddressIpAddressDoesntExistIpAddressCorrectlyAdded()
        {
            IpAddressEntity ipAddress = new IpAddressEntity("127.0.0.1", "0x7F000001");
            IpAddressEntity ipAddressFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    ipAddressFromDao = await _ipAddressDao.Add(ipAddress, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(ipAddressFromDao.Ip, Is.EqualTo(ipAddress.Ip));
            Assert.That(ipAddressFromDao.BinaryIp, Is.EqualTo(ipAddress.BinaryIp));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM ip_address"))
            {
                while (reader.Read())
                {
                    count++;

                    Assert.That(reader.GetInt64("id"), Is.EqualTo(ipAddressFromDao.Id));
                    Assert.That(reader.GetString("address"), Is.EqualTo(ipAddressFromDao.Ip));
                    Assert.That(Encoding.UTF8.GetString(reader.GetByteArray("binary_address")), Is.EqualTo(ipAddressFromDao.BinaryIp));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddIpAddressIpAddressAlreadyExistsIpAddressNotAddedCorrectIdReturned()
        {
            IpAddressEntity ipAddress = new IpAddressEntity("127.0.0.1", "0x7F000001");
            IpAddressEntity ipAddressFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _ipAddressDao.Add(ipAddress, connection, transaction);
                    ipAddressFromDao = await _ipAddressDao.Add(ipAddress, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(ipAddressFromDao.Ip, Is.EqualTo(ipAddress.Ip));
            Assert.That(ipAddressFromDao.BinaryIp, Is.EqualTo(ipAddress.BinaryIp));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM ip_address"))
            {
                while (reader.Read())
                {
                    count++;

                    Assert.That(reader.GetInt64("id"), Is.EqualTo(ipAddressFromDao.Id));
                    Assert.That(reader.GetString("address"), Is.EqualTo(ipAddressFromDao.Ip));
                    Assert.That(Encoding.UTF8.GetString(reader.GetByteArray("binary_address")), Is.EqualTo(ipAddressFromDao.BinaryIp));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }
    }
}
