using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderTextValue;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class Rfc822HeaderTextValueDaoTests : DatabaseTestBase
    {
        private Rfc822HeaderTextValueDao _rfc822HeaderTextValueDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _rfc822HeaderTextValueDao = new Rfc822HeaderTextValueDao();
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddRfc822HeaderTextValueWhenDoenstExistsCorrectlyAdded()
        {
            Rfc822HeaderTextValueEntity rfc822HeaderTextValue = new Rfc822HeaderTextValueEntity("text");
            Rfc822HeaderTextValueEntity rfc822HeaderTextValueFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    rfc822HeaderTextValueFromDao = await _rfc822HeaderTextValueDao.Add(rfc822HeaderTextValue, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(rfc822HeaderTextValueFromDao.Value, Is.EqualTo(rfc822HeaderTextValue.Value));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM rfc822_header_text_values"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("id"), Is.EqualTo(rfc822HeaderTextValue.Id));
                    Assert.That(reader.GetString("value"), Is.EqualTo(rfc822HeaderTextValue.Value));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddRfc822HeaderTextValueWhenAlreadyExistsReturnsCorrectValues()
        {
            Rfc822HeaderTextValueEntity rfc822HeaderTextValue = new Rfc822HeaderTextValueEntity("text");
            Rfc822HeaderTextValueEntity rfc822HeaderTextValueFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _rfc822HeaderTextValueDao.Add(rfc822HeaderTextValue, connection, transaction);
                    rfc822HeaderTextValueFromDao = await _rfc822HeaderTextValueDao.Add(rfc822HeaderTextValue, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(rfc822HeaderTextValueFromDao.Value, Is.EqualTo(rfc822HeaderTextValue.Value));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM rfc822_header_text_values"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("id"), Is.EqualTo(rfc822HeaderTextValue.Id));
                    Assert.That(reader.GetString("value"), Is.EqualTo(rfc822HeaderTextValue.Value));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }
    }
}
