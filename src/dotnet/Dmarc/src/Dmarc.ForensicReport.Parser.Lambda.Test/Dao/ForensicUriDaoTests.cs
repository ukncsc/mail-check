using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicUri;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class ForensicUriDaoTests : DatabaseTestBase
    {
        private ForensicUriDao _forensicUriDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _forensicUriDao = new ForensicUriDao();
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddForensicUriWhenDoesntExistCorrectlyAdded()
        {
            ForensicUriEntity forensicUri = new ForensicUriEntity("http://domain.com", "0x1f2d");
            ForensicUriEntity forensicUriFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    forensicUriFromDao = await _forensicUriDao.Add(forensicUri, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicUriFromDao.Uri, Is.EqualTo(forensicUri.Uri));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_uri"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(forensicUriFromDao.Id));
                    Assert.That(reader.GetString("uri"), Is.EqualTo(forensicUriFromDao.Uri));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddForensicUriWhenAlreadyExistsReturnsCorrectValues()
        {
            ForensicUriEntity forensicUri = new ForensicUriEntity("http://domain.com", "0x1f2d");
            ForensicUriEntity forensicUriFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _forensicUriDao.Add(forensicUri, connection, transaction);
                    forensicUriFromDao = await _forensicUriDao.Add(forensicUri, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicUriFromDao.Uri, Is.EqualTo(forensicUri.Uri));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_uri"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(forensicUriFromDao.Id));
                    Assert.That(reader.GetString("uri"), Is.EqualTo(forensicUriFromDao.Uri));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }
    }
}
