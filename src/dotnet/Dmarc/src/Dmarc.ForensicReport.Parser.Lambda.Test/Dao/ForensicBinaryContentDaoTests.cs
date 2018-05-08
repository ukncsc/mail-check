using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinaryContent;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinaryHash;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Utils;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class ForensicBinaryContentDaoTests : DatabaseTestBase
    {
        private ForensicBinaryContentDao _forensicBinaryContentDao;
        private IForensicBinaryHashDao _forensicBinaryHashDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _forensicBinaryHashDao = new ForensicBinaryHashDao();
            _forensicBinaryContentDao = new ForensicBinaryContentDao(_forensicBinaryHashDao);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddForensicBinaryContentWhenDoesntExistCorrectlyAdded()
        {
            ForensicBinaryContentEntity forensicBinaryContent = new ForensicBinaryContentEntity(new byte[] {0x20, 0x20, 0x20, 0x20 }, new List<HashEntity> {new HashEntity(EntityHashType.Sha1, "A2D3F2==")});
            ForensicBinaryContentEntity forensicBinaryContentFromDao;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    forensicBinaryContentFromDao = await _forensicBinaryContentDao.Add(forensicBinaryContent, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicBinaryContentFromDao.Content, Is.EqualTo(forensicBinaryContent.Content));


            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_binary"))
            {
                while (reader.Read())
                {
                    count++;

                    Assert.That(reader.GetByteArray("attachment"), Is.EqualTo(forensicBinaryContentFromDao.Content));
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(forensicBinaryContentFromDao.Id));
                }
            }
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddForensicBinaryContentWhenAlreadyExistsReturnsCorrectValues()
        {
            ForensicBinaryContentEntity forensicBinaryContent = new ForensicBinaryContentEntity(new byte[] { 0x20, 0x20, 0x20, 0x20 }, new List<HashEntity> { new HashEntity(EntityHashType.Sha1, "A2D3F2==") });
            ForensicBinaryContentEntity forensicBinaryContentFromDao;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _forensicBinaryContentDao.Add(forensicBinaryContent, connection, transaction);
                    forensicBinaryContentFromDao = await _forensicBinaryContentDao.Add(forensicBinaryContent, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicBinaryContentFromDao.Content, Is.EqualTo(forensicBinaryContent.Content));


            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_binary"))
            {
                while (reader.Read())
                {
                    count++;

                    Assert.That(reader.GetByteArray("attachment"), Is.EqualTo(forensicBinaryContentFromDao.Content));
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(forensicBinaryContentFromDao.Id));
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
                    ForensicBinaryContentEntity forensicBinaryContent = new ForensicBinaryContentEntity(new byte[] { 0x20, 0x20, 0x20, 0x20 }, new List<HashEntity>());
                    Assert.ThrowsAsync<InvalidOperationException>(async () => await _forensicBinaryContentDao.Add(forensicBinaryContent, connection, transaction));
                }
            }
        }
    }
}
