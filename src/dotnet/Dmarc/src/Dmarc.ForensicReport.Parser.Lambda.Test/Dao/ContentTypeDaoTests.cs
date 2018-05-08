using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ContentType;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class ContentTypeDaoTests : DatabaseTestBase
    {
        private ContentTypeDao _contentTypeDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _contentTypeDao = new ContentTypeDao();
        }

        [Test]
        public async Task AddContentTypeWhenDoesntExistCorrectlyAdded()
        {
            ContentTypeEntity contentType = new ContentTypeEntity("text/plain");
            ContentTypeEntity contentTypesFromDao;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    contentTypesFromDao = await _contentTypeDao.Add(contentType, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(contentTypesFromDao.Name, Is.EqualTo(contentType.Name));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM content_type"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetString("name"), Is.EqualTo(contentTypesFromDao.Name));
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(contentTypesFromDao.Id));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddContentTypeWhenAlreadyExistsReturnCorrectValues()
        {
            ContentTypeEntity contentType = new ContentTypeEntity("text/plain");
            ContentTypeEntity contentTypesFromDao;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _contentTypeDao.Add(contentType, connection, transaction);
                    contentTypesFromDao = await _contentTypeDao.Add(contentType, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(contentTypesFromDao.Name, Is.EqualTo(contentType.Name));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM content_type"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetString("name"), Is.EqualTo(contentTypesFromDao.Name));
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(contentTypesFromDao.Id));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }


        //        [Test]
        //        public async Task AddContentTypeWhenDoesntExistCorrectlyAdded()
        //        {
        //            ContentTypeEntity contentType = new ContentTypeEntity("text/plain");
        //
        //            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //            {
        //                await connection.OpenAsync().ConfigureAwait(false);
        //                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
        //                {
        //                    List<ContentTypeEntity> contentTypesFromDao = await _contentTypeDao.Add(new List<ContentTypeEntity> { contentType }, connection, transaction);
        //                    List<Tuple<int, string>> contentTypesFromDatabase = await GetValues(connection, transaction);
        //
        //                    Assert.That(contentTypesFromDao.Count, Is.EqualTo(1));
        //                    Assert.That(contentTypesFromDao[0].Name, Is.EqualTo(contentType.Name));
        //
        //                    Assert.That(contentTypesFromDatabase.Count, Is.EqualTo(1));
        //                    Assert.That(contentTypesFromDatabase[0].Item2, Is.EqualTo(contentType.Name));
        //
        //                    Assert.That(contentTypesFromDao[0].Id, Is.EqualTo(contentTypesFromDatabase[0].Item1));
        //                }
        //            }
        //        }
        //
        //        [Test]
        //        public async Task AddContentTypeWhenDoesExistReturnsExpectedRecord()
        //        {
        //            ContentTypeEntity contentType = new ContentTypeEntity("text/plain");
        //
        //            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //            {
        //                await connection.OpenAsync().ConfigureAwait(false);
        //                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
        //                {
        //                    List<ContentTypeEntity> contentTypesFromDao1 = await _contentTypeDao.Add(new List<ContentTypeEntity> { contentType }, connection, transaction);
        //
        //                    List<Tuple<int, string>> contentTypesFromDatabase = await GetValues(connection, transaction);
        //
        //                    List<ContentTypeEntity> contentTypesFromDao2 = await _contentTypeDao.Add(new List<ContentTypeEntity> { contentType }, connection, transaction);
        //
        //                    Assert.That(contentTypesFromDao2.Count, Is.EqualTo(1));
        //                    Assert.That(contentTypesFromDao2[0].Name, Is.EqualTo(contentType.Name));
        //
        //                    Assert.That(contentTypesFromDatabase.Count, Is.EqualTo(1));
        //                    Assert.That(contentTypesFromDatabase[0].Item2, Is.EqualTo(contentType.Name));
        //
        //                    Assert.That(contentTypesFromDao2[0].Id, Is.EqualTo(contentTypesFromDatabase[0].Item1));
        //                }
        //            }
        //        }
        //
        //        [Test]
        //        public async Task AddContentTypeWhenOneExistsAndOneDoesntExistCorrectlyAdded()
        //        {
        //            ContentTypeEntity contentType1 = new ContentTypeEntity("text/plain");
        //            ContentTypeEntity contentType2 = new ContentTypeEntity("text/html");
        //
        //            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //            {
        //                await connection.OpenAsync().ConfigureAwait(false);
        //                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
        //                {
        //                    List<ContentTypeEntity> contentTypesFromDao1 = await _contentTypeDao.Add(new List<ContentTypeEntity> { contentType1 }, connection, transaction);
        //
        //                    List<ContentTypeEntity> contentTypesFromDao2 = await _contentTypeDao.Add(new List<ContentTypeEntity> { contentType1, contentType2 }, connection, transaction);
        //
        //                    List<Tuple<int, string>> contentTypesFromDatabase = await GetValues(connection, transaction);
        //
        //                    Assert.That(contentTypesFromDao2.Count, Is.EqualTo(2));
        //                    Assert.That(contentTypesFromDao2[0].Name, Is.EqualTo(contentType1.Name));
        //                    Assert.That(contentTypesFromDao2[1].Name, Is.EqualTo(contentType2.Name));
        //
        //                    Assert.That(contentTypesFromDatabase.Count, Is.EqualTo(2));
        //                    Assert.That(contentTypesFromDatabase[0].Item2, Is.EqualTo(contentType1.Name));
        //                    Assert.That(contentTypesFromDatabase[1].Item2, Is.EqualTo(contentType2.Name));
        //
        //                    Assert.That(contentTypesFromDao2[0].Id, Is.EqualTo(contentTypesFromDatabase[0].Item1));
        //                    Assert.That(contentTypesFromDao2[1].Id, Is.EqualTo(contentTypesFromDatabase[1].Item1));
        //                }
        //            }
        //        }

        //private async Task<List<Tuple<int, string>>> GetValues(MySqlConnection connection, MySqlTransaction transaction)
        //{
        //    MySqlCommand command = new MySqlCommand("SELECT `id`,`name` FROM `content_type` ORDER BY `id`;", connection, transaction);
        //    List<Tuple<int, string>> values = new List<Tuple<int, string>>();
        //    using (DbDataReader reader = await command.ExecuteReaderAsync())
        //    {
        //        while (await reader.ReadAsync())
        //        {
        //            values.Add(new Tuple<int, string>(reader.GetInt32("id"), reader.GetString("name")));
        //        }
        //    }
        //    return values;
        //}
    }
}
