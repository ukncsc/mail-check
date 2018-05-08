using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderField;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class Rfc822HeaderFieldDaoTests : DatabaseTestBase
    {
        private Rfc822HeaderFieldDao _rfc822HeaderFieldDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            
            _rfc822HeaderFieldDao = new Rfc822HeaderFieldDao();
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddRfc822HeaderFieldWhenDoenstExistCorrectlyAdded()
        {
            Rfc822HeaderFieldEntity headerField = new Rfc822HeaderFieldEntity("To", EntityRfc822HeaderValueType.Email);
            Rfc822HeaderFieldEntity headerFieldFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    headerFieldFromDao = await _rfc822HeaderFieldDao.Add(headerField, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(headerFieldFromDao.Name, Is.EqualTo(headerField.Name));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM rfc822_header_fields;"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(headerFieldFromDao.Id));
                    Assert.That(reader.GetString("value_type"), Is.EqualTo(headerFieldFromDao.ValueType.GetDbName()));
                    Assert.That(reader.GetString("name"), Is.EqualTo(headerFieldFromDao.Name));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddRfc822HeaderFieldWhenAlreadyExistsReturnsCorrectValues()
        {
            Rfc822HeaderFieldEntity headerField = new Rfc822HeaderFieldEntity("To", EntityRfc822HeaderValueType.Email);
            Rfc822HeaderFieldEntity headerFieldFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _rfc822HeaderFieldDao.Add(headerField, connection, transaction);
                    headerFieldFromDao = await _rfc822HeaderFieldDao.Add(headerField, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(headerFieldFromDao.Name, Is.EqualTo(headerField.Name));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM rfc822_header_fields;"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt32("id"), Is.EqualTo(headerFieldFromDao.Id));
                    Assert.That(reader.GetString("value_type"), Is.EqualTo(headerFieldFromDao.ValueType.GetDbName()));
                    Assert.That(reader.GetString("name"), Is.EqualTo(headerFieldFromDao.Name));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

    }
}
