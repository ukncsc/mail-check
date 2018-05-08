using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextContent;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextContentUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextHash;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Utils;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class ForensicTextContentDaoTests : DatabaseTestBase
    {
        private ForensicTextContentDao _forensicTextContentDao;
        private ForensicTextHashDao _forensicTextHashDao;
        private ForensicTextContentUriDao _forensicTextContentUriDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _forensicTextHashDao = new ForensicTextHashDao();
            _forensicTextContentUriDao = new ForensicTextContentUriDao(new ForensicUriDao());
            _forensicTextContentDao = new ForensicTextContentDao(_forensicTextHashDao, _forensicTextContentUriDao);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddForensicTextContentWhenDoesntExistCorrectlyAdded()
        {
            ForensicTextContentEntity forensicTextContentEntity = Create();
            ForensicTextContentEntity forensicTextContentEntityFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    forensicTextContentEntityFromDao = await _forensicTextContentDao.Add(forensicTextContentEntity, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicTextContentEntityFromDao.Text, Is.EqualTo(forensicTextContentEntity.Text));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_text"))
            {
                while (reader.Read())
                {
                    count++;

                    Assert.That(reader.GetString("body"), Is.EqualTo(forensicTextContentEntityFromDao.Text));
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(forensicTextContentEntityFromDao.Id));
                }
            }
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddForensicTextContentWhenAlreadyExistsReturnsCorrectValues()
        {
            ForensicTextContentEntity forensicTextContentEntity = Create();
            ForensicTextContentEntity forensicTextContentEntityFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _forensicTextContentDao.Add(forensicTextContentEntity, connection, transaction);
                    forensicTextContentEntityFromDao = await _forensicTextContentDao.Add(forensicTextContentEntity, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicTextContentEntityFromDao.Text, Is.EqualTo(forensicTextContentEntity.Text));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_text"))
            {
                while (reader.Read())
                {
                    count++;

                    Assert.That(reader.GetString("body"), Is.EqualTo(forensicTextContentEntityFromDao.Text));
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(forensicTextContentEntityFromDao.Id));
                }
            }
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task ThrowsIfNoSha1HashAvailable()
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    ForensicTextContentEntity forensicTextContentEntity = Create(false);
                    Assert.ThrowsAsync<InvalidOperationException>(async () => await _forensicTextContentDao.Add(forensicTextContentEntity, connection, transaction));
                }
            }
        }

        private ForensicTextContentEntity Create(bool includeHashes = true)
        {
            List<HashEntity> hashEntities = includeHashes ? new List<HashEntity> {new HashEntity(EntityHashType.Sha1, "A34RFWW43==")} : new List<HashEntity>();
            List<ForensicTextContentUriEntity> forensicTextContentUriEntities = new List<ForensicTextContentUriEntity> {new ForensicTextContentUriEntity(new ForensicUriEntity("http://domain.com", "a1b2c3"))};
            return new ForensicTextContentEntity("Forensic Text", hashEntities, forensicTextContentUriEntities);
        }
    }
}
