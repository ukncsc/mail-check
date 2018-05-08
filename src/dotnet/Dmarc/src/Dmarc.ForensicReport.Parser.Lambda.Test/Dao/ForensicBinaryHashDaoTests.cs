using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinaryHash;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Utils;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class ForensicBinaryHashDaoTests : DatabaseTestBase
    {
        private ForensicBinaryHashDao _forensicBinaryHashDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _forensicBinaryHashDao = new ForensicBinaryHashDao();
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddHashWhenDoesntExistCorrectlyAdded()
        {
            long forensicBinaryContentId = (long)(ulong)MySqlHelper.ExecuteScalar(ConnectionString, "INSERT INTO forensic_binary(`attachment`) VALUES(x'020202'); SELECT LAST_INSERT_ID();");

            HashEntity hashEntity = new HashEntity(EntityHashType.Sha1, "A4D33FG==") {ContentId = forensicBinaryContentId};
            HashEntity hashEntityFromDao;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    hashEntityFromDao = await _forensicBinaryHashDao.Add(hashEntity, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }
            Assert.That(hashEntityFromDao.Hash, Is.EqualTo(hashEntity.Hash));
            Assert.That(hashEntityFromDao.Type, Is.EqualTo(hashEntity.Type));


            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_binary_hash"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("binary_id"), Is.EqualTo(hashEntityFromDao.ContentId));
                    Assert.That(reader.GetString("type"), Is.EqualTo(hashEntityFromDao.Type.GetDbName()));
                    Assert.That(reader.GetString("hash"), Is.EqualTo(hashEntityFromDao.Hash));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddHashWhenAlreadyExistsReturnsCorrectValues()
        {
            long forensicBinaryContentId = (long)(ulong)MySqlHelper.ExecuteScalar(ConnectionString, "INSERT INTO forensic_binary(`attachment`) VALUES(x'020202'); SELECT LAST_INSERT_ID();");

            HashEntity hashEntity = new HashEntity(EntityHashType.Sha1, "A4D33FG==") { ContentId = forensicBinaryContentId };
            HashEntity hashEntityFromDao;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _forensicBinaryHashDao.Add(hashEntity, connection, transaction);
                    hashEntityFromDao = await _forensicBinaryHashDao.Add(hashEntity, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }
            Assert.That(hashEntityFromDao.Hash, Is.EqualTo(hashEntity.Hash));
            Assert.That(hashEntityFromDao.Type, Is.EqualTo(hashEntity.Type));


            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_binary_hash"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("binary_id"), Is.EqualTo(hashEntityFromDao.ContentId));
                    Assert.That(reader.GetString("type"), Is.EqualTo(hashEntityFromDao.Type.GetDbName()));
                    Assert.That(reader.GetString("hash"), Is.EqualTo(hashEntityFromDao.Hash));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }
    }
}
