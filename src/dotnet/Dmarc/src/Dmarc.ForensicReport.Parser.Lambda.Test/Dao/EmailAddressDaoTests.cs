using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.EmailAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class EmailAddressDaoTests : DatabaseTestBase
    {
        private EmailAddressDao _emailAddressDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _emailAddressDao = new EmailAddressDao();
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddEmailAddressWhenDoesntExistCorrectlyAdded()
        {
            EmailAddressEntity emailAddress = new EmailAddressEntity("test@gov.uk");
            EmailAddressEntity emailAddressFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    emailAddressFromDao = await _emailAddressDao.Add(emailAddress, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(emailAddressFromDao.EmailAddress, Is.EqualTo(emailAddress.EmailAddress));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM email_address"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetString("address"), Is.EqualTo(emailAddressFromDao.EmailAddress));
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(emailAddressFromDao.Id));
                }
            }

            Assert.That(count, Is.EqualTo(1));

        }

        [Test]
        public async Task AddEmailAddressWhenAlreadyExistsReturnsCorrectValues()
        {
            EmailAddressEntity emailAddress = new EmailAddressEntity("test@gov.uk");
            EmailAddressEntity emailAddressFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _emailAddressDao.Add(emailAddress, connection, transaction);
                    emailAddressFromDao = await _emailAddressDao.Add(emailAddress, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(emailAddressFromDao.EmailAddress, Is.EqualTo(emailAddress.EmailAddress));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM email_address"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetString("address"), Is.EqualTo(emailAddressFromDao.EmailAddress));
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(emailAddressFromDao.Id));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }
    }
}
