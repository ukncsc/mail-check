using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextHash;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Utils;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class ForensicTextHashDaoTests : DatabaseTestBase
    {
        private ForensicTextHashDao _forensicTextHashDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _forensicTextHashDao = new ForensicTextHashDao();
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddHashWhenDoesntExistCorrectlyAdded()
        {
            long forensicTextContentId = (long)(ulong)MySqlHelper.ExecuteScalar(ConnectionString, "INSERT INTO `forensic_text` (`body`) VALUES(''); SELECT LAST_INSERT_ID();");

            HashEntity hashEntity = new HashEntity(EntityHashType.Sha1, "A4D33FG==") { ContentId = forensicTextContentId };
            HashEntity hashEntityFromDao;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    hashEntityFromDao = await _forensicTextHashDao.Add(hashEntity, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }
            Assert.That(hashEntityFromDao.Hash, Is.EqualTo(hashEntity.Hash));
            Assert.That(hashEntityFromDao.Type, Is.EqualTo(hashEntity.Type));


            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_text_hash"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("text_id"), Is.EqualTo(hashEntityFromDao.ContentId));
                    Assert.That(reader.GetString("type"), Is.EqualTo(hashEntityFromDao.Type.GetDbName()));
                    Assert.That(reader.GetString("hash"), Is.EqualTo(hashEntityFromDao.Hash));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddHashWhenAlreadyExistsReturnsCorrectValues()
        {
            long forensicTextContentId = (long)(ulong)MySqlHelper.ExecuteScalar(ConnectionString, "INSERT INTO `forensic_text` (`body`) VALUES(''); SELECT LAST_INSERT_ID();");

            HashEntity hashEntity = new HashEntity(EntityHashType.Sha1, "A4D33FG==") { ContentId = forensicTextContentId };
            HashEntity hashEntityFromDao;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _forensicTextHashDao.Add(hashEntity, connection, transaction);
                    hashEntityFromDao = await _forensicTextHashDao.Add(hashEntity, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }
            Assert.That(hashEntityFromDao.Hash, Is.EqualTo(hashEntity.Hash));
            Assert.That(hashEntityFromDao.Type, Is.EqualTo(hashEntity.Type));


            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_text_hash"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("text_id"), Is.EqualTo(hashEntityFromDao.ContentId));
                    Assert.That(reader.GetString("type"), Is.EqualTo(hashEntityFromDao.Type.GetDbName()));
                    Assert.That(reader.GetString("hash"), Is.EqualTo(hashEntityFromDao.Hash));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }
    }
}
