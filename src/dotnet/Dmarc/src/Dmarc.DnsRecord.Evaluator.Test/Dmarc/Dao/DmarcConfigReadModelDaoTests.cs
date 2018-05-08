using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.Common.TestSupport;
using Dmarc.DnsRecord.Evaluator.Dmarc.Dao;
using Dmarc.DnsRecord.Evaluator.Dmarc.Dao.Entities;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class DmarcConfigReadModelDaoTests : DatabaseTestBase
    {
        private DmarcConfigReadModelDao _dmarcConfigReadModelDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            IConnectionInfoAsync connectionInfoAsync = A.Fake<IConnectionInfoAsync>();
            _dmarcConfigReadModelDao = new DmarcConfigReadModelDao(connectionInfoAsync, A.Fake<ILogger>());

            A.CallTo(() => connectionInfoAsync.GetConnectionStringAsync()).Returns(Task.FromResult(ConnectionString));
        }

        [Test]
        public async Task InsertNewRecords()
        {
            ulong domain1Id = CreateDomain("Domain1");
            ulong domain2Id = CreateDomain("Domain2");

            List<DmarcConfigReadModelEntity> readModels = new List<DmarcConfigReadModelEntity>
            {
                new DmarcConfigReadModelEntity((int)domain1Id, 1, ErrorType.Warning, "readmodel1"),
                new DmarcConfigReadModelEntity((int)domain2Id, 2, ErrorType.Error, "readmodel2")
            };

            await _dmarcConfigReadModelDao.InsertOrUpdate(readModels);

            List<DmarcConfigReadModelEntity> entities = GetAllRecordEntities();

            Assert.That(entities.SequenceEqual(readModels), Is.True);
        }

        [Test]
        public async Task UpdateExistingRecords()
        {
            ulong domain1Id = CreateDomain("Domain1");
            ulong domain2Id = CreateDomain("Domain2");

            List<DmarcConfigReadModelEntity> readModels1 = new List<DmarcConfigReadModelEntity>
            {
                new DmarcConfigReadModelEntity((int)domain1Id, 1, ErrorType.Warning, "readmodel1"),
                new DmarcConfigReadModelEntity((int)domain2Id, 2, ErrorType.Error, "readmodel2")
            };

            await _dmarcConfigReadModelDao.InsertOrUpdate(readModels1);

            List<DmarcConfigReadModelEntity> readModels2 = new List<DmarcConfigReadModelEntity>
            {
                new DmarcConfigReadModelEntity((int)domain1Id, 2, ErrorType.Error, "readmodel12"),
                new DmarcConfigReadModelEntity((int)domain2Id, 3, ErrorType.Warning, "readmodel22")
            };

            await _dmarcConfigReadModelDao.InsertOrUpdate(readModels2);

            List<DmarcConfigReadModelEntity> entities = GetAllRecordEntities();

            Assert.That(entities.SequenceEqual(readModels2), Is.True);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        #region Test Support
        private ulong CreateDomain(string domainName)
        {
            MySqlHelper.ExecuteNonQuery(ConnectionString, $@"INSERT INTO `domain`(`name`, `created_by`) VALUES('{domainName}', 'test');");
            return (ulong)MySqlHelper.ExecuteScalar(ConnectionString, "SELECT LAST_INSERT_ID();");
        }

        private List<DmarcConfigReadModelEntity> GetAllRecordEntities()
        {
            List<DmarcConfigReadModelEntity> entities = new List<DmarcConfigReadModelEntity>();
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM dns_record_dmarc_read_model;"))
            {
                while (reader.Read())
                {
                    DmarcConfigReadModelEntity dmarcConfigReadModelEntity = 
                        new DmarcConfigReadModelEntity(
                            (int)reader.GetInt64("domain_id"),
                            reader.GetInt32("error_count"),
                            (ErrorType)Enum.Parse(typeof(ErrorType), reader.GetString("max_error_severity"), true),
                            reader.GetString("model"));
                
                    entities.Add(dmarcConfigReadModelEntity);
                }
            }
            return entities;
        }
        #endregion Test Support
    }
}
