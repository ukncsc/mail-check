using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextContentUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicUri;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class ForensicTextContentUriDaoTests : DatabaseTestBase
    {
        private ForensicTextContentUriDao _forensicTextContentUriDao;
        private ForensicUriDao _forensicUriDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _forensicUriDao = new ForensicUriDao();
            _forensicTextContentUriDao = new ForensicTextContentUriDao(_forensicUriDao);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddForensicTextContentUriWhenDoesntExistCorrectlyAdded()
        {
            long fornsicTextId = GetForensicTextId();

            ForensicTextContentUriEntity forensicTextContentUriEntity = new ForensicTextContentUriEntity(new ForensicUriEntity("http://domain.com", "a1b2c3")) {ForensicTextContentId = fornsicTextId};
            List<ForensicTextContentUriEntity> forensicTextContentUriEntitisFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    forensicTextContentUriEntitisFromDao = await _forensicTextContentUriDao.Add(new List<ForensicTextContentUriEntity> {forensicTextContentUriEntity}, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicTextContentUriEntitisFromDao.Count, Is.EqualTo(1));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_uri_match"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("forensic_text_id"), Is.EqualTo(forensicTextContentUriEntitisFromDao[0].ForensicTextContentId));
                    Assert.That(reader.GetInt32("uri_id"), Is.EqualTo(forensicTextContentUriEntitisFromDao[0].ForensicUri.Id));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddMultipleForensicTextContentUrisWhenDontExistCorrectlyAdded()
        {
            long fornsicTextId = GetForensicTextId();

            ForensicTextContentUriEntity forensicTextContentUriEntity1 = new ForensicTextContentUriEntity(new ForensicUriEntity("http://domain1.com", "a1b2c3")) { ForensicTextContentId = fornsicTextId };
            ForensicTextContentUriEntity forensicTextContentUriEntity2 = new ForensicTextContentUriEntity(new ForensicUriEntity("http://domain2.com", "a1b2c4")) { ForensicTextContentId = fornsicTextId };
            List<ForensicTextContentUriEntity> forensicTextContentUriEntitiesFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    forensicTextContentUriEntitiesFromDao = await _forensicTextContentUriDao.Add(new List<ForensicTextContentUriEntity> { forensicTextContentUriEntity1, forensicTextContentUriEntity2 }, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicTextContentUriEntitiesFromDao.Count, Is.EqualTo(2));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_uri_match"))
            {
                while (reader.Read())
                {
                    Assert.That(reader.GetInt64("forensic_text_id"), Is.EqualTo(forensicTextContentUriEntitiesFromDao[count].ForensicTextContentId));
                    Assert.That(reader.GetInt32("uri_id"), Is.EqualTo(forensicTextContentUriEntitiesFromDao[count].ForensicUri.Id));
                    count++;
                }
            }

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public async Task ThrowsIfInsertedTwice()
        {
            long fornsicTextId = GetForensicTextId();

            ForensicTextContentUriEntity forensicTextContentUriEntity = new ForensicTextContentUriEntity(new ForensicUriEntity("http://domain.com", "a1b2c3")) { ForensicTextContentId = fornsicTextId };

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _forensicTextContentUriDao.Add(new List<ForensicTextContentUriEntity> { forensicTextContentUriEntity }, connection, transaction);
                    Assert.ThrowsAsync<MySqlException>(async() => await _forensicTextContentUriDao.Add(new List<ForensicTextContentUriEntity> { forensicTextContentUriEntity }, connection, transaction));
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        private long GetForensicTextId()
        {
            return (long)(ulong)MySqlHelper.ExecuteScalar(ConnectionString, "INSERT INTO `forensic_text` (`body`) VALUES('body'); SELECT LAST_INSERT_ID();");
        }
    }
}
