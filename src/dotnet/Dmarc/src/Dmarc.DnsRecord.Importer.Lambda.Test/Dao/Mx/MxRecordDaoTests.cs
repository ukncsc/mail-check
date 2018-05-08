using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.TestSupport;
using Dmarc.DnsRecord.Importer.Lambda.Config;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Mx;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using FakeItEasy;
using Heijden.DNS;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Importer.Lambda.Test.Dao.Mx
{
    [TestFixture]
    [Category("Integration")]
    public class MxRecordDaoTests : DatabaseTestBase
    {
        private const string Domain = "gov.uk";
        private const string Host1 = "hostname1";
        private const string Host2 = "hostname2";
        private const int Preference1 = 1;
        private const int Preference2 = 2;

        private MxRecordDao _mxRecordDao;
        private IConnectionInfoAsync _connectionInfoAsync;
        private IRecordImporterConfig _recordImporterConfig;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _connectionInfoAsync = A.Fake<IConnectionInfoAsync>();
            _recordImporterConfig = A.Fake<IRecordImporterConfig>();
            _mxRecordDao = new MxRecordDao(_connectionInfoAsync, _recordImporterConfig, A.Fake<ILogger>());

            A.CallTo(() => _connectionInfoAsync.GetConnectionStringAsync()).Returns(Task.FromResult(ConnectionString));
        }

        [Test]
        public async Task RetrievesNoRecordsWhenNoNsRecordsExist()
        {
            CreateDomain(Domain);

            SetConfig(_recordImporterConfig, 10, 1, 10);

            Dictionary<DomainEntity, List<RecordEntity>> domainEntities = await _mxRecordDao.GetRecordsForUpdate();

            Assert.That(domainEntities.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task RetrievesEmptyRecordsToUpdateWhenNoRecordsExist()
        {
            var domainId = CreateDomain(Domain);
            CreateNsRecord(Host1, domainId, DateTime.UtcNow);

            SetConfig(_recordImporterConfig, 10, 1, 10);

            Dictionary<DomainEntity, List<RecordEntity>> domainEntities = await _mxRecordDao.GetRecordsForUpdate();

            Assert.That(domainEntities.Keys.Count, Is.EqualTo(1));
            Assert.That(domainEntities.Keys.First().Name, Is.EqualTo(Domain));
            Assert.That(domainEntities.Values.First().Count, Is.EqualTo(0));
        }

        [Test]
        public async Task RetrievesRecordsToUpdateWhenSuccessfulRecordsNeedUpdating()
        {
            int successInterval = 10;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(successInterval + 1));

            ulong domainId = CreateDomain(Domain);
            CreateNsRecord(Host1, domainId, earlier);
            CreateMxRecord(Preference1, Host1, domainId, earlier);

            SetConfig(_recordImporterConfig, successInterval, 1, 10);

            Dictionary<DomainEntity, List<RecordEntity>> domainEntities = await _mxRecordDao.GetRecordsForUpdate();

            Assert.That(domainEntities.Values.First().Count, Is.EqualTo(1));
            Assert.That(domainEntities.Values.First().First().Domain.Name, Is.EqualTo(Domain));
            Assert.That(((MxRecordInfo)domainEntities.Values.First().First().RecordInfo).Preference, Is.EqualTo(Preference1));
            Assert.That(((MxRecordInfo)domainEntities.Values.First().First().RecordInfo).Host, Is.EqualTo(Host1));
        }

        [Test]
        public async Task RetrievesNoRecordsToUpdateWhenSuccessfulRecordsDontNeedUpdating()
        {
            int successInterval = 10;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(successInterval - 1));

            ulong domainId = CreateDomain(Domain);
            CreateNsRecord(Host1, domainId, earlier);
            CreateMxRecord(Preference1, Host1, domainId, earlier);

            SetConfig(_recordImporterConfig, successInterval, 1, 10);

            Dictionary<DomainEntity, List<RecordEntity>> domainEntities = await _mxRecordDao.GetRecordsForUpdate();

            Assert.That(domainEntities, Is.Empty);
        }

        [Test]
        public async Task RetrievesRecordsToUpdateWhenUnsuccessfulRecordsNeedUpdating()
        {
            int failureInterval = 10;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(failureInterval + 1));

            ulong domainId = CreateDomain(Domain);
            CreateNsRecord(Host1, domainId, earlier);
            CreateMxRecord(Preference1, Host1, domainId, earlier, 1);

            SetConfig(_recordImporterConfig, 100, failureInterval, 10);

            Dictionary<DomainEntity, List<RecordEntity>> domainEntities = await _mxRecordDao.GetRecordsForUpdate();

            Assert.That(domainEntities.Count, Is.EqualTo(1));
            Assert.That(domainEntities.Values.First().First().Domain.Name, Is.EqualTo(Domain));
            Assert.That(((MxRecordInfo)domainEntities.Values.First().First().RecordInfo).Host, Is.EqualTo(Host1));
            Assert.That(((MxRecordInfo)domainEntities.Values.First().First().RecordInfo).Preference, Is.EqualTo(Preference1));
        }

        [Test]
        public async Task RetrievesNoRecordsToUpdateWhenUnsuccessfulRecordsDontNeedUpdating()
        {
            int failureInterval = 10;

            DateTime earlier = DateTime.UtcNow.AddSeconds(-(failureInterval - 1));

            ulong domainId = CreateDomain(Domain);
            CreateNsRecord(Host1, domainId, earlier);
            CreateMxRecord(Preference1, Host1, domainId, earlier, 1);

            SetConfig(_recordImporterConfig, 100, failureInterval, 10);

            Dictionary<DomainEntity, List<RecordEntity>> domainEntities = await _mxRecordDao.GetRecordsForUpdate();

            Assert.That(domainEntities, Is.Empty);
        }

        [Test]
        public async Task CorrectlyInsertsNewRecords()
        {
            ulong domainId = CreateDomain(Domain);

            List<RecordEntity> recordEntities = new List<RecordEntity>
            {
                new RecordEntity(null, new DomainEntity((int)domainId, Domain), new MxRecordInfo(Host1, Preference1), RCode.NoError, 0),
                new RecordEntity(null, new DomainEntity((int)domainId, Domain), new MxRecordInfo(Host2, Preference2), RCode.NoError, 0)
            };

            await _mxRecordDao.InsertOrUpdateRecords(recordEntities);

            List<MxRecord> recordEntitiesFromDb = GetAllRecordEntities();

            Assert.That(recordEntitiesFromDb.Count, Is.EqualTo(2));

            Assert.That(recordEntitiesFromDb[0].DomainId, Is.EqualTo(domainId));
            Assert.That(recordEntitiesFromDb[0].Preference, Is.EqualTo(Preference1));
            Assert.That(recordEntitiesFromDb[0].Hostname, Is.EqualTo(Host1));
            Assert.That(recordEntitiesFromDb[0].StartDate, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[0].EndDate, Is.Null);
            Assert.That(recordEntitiesFromDb[0].LastChecked, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[0].FailureCount, Is.EqualTo(0));
            Assert.That(recordEntitiesFromDb[0].ResultCode, Is.EqualTo(0));

            Assert.That(recordEntitiesFromDb[1].DomainId, Is.EqualTo(domainId));
            Assert.That(recordEntitiesFromDb[1].Preference, Is.EqualTo(Preference2));
            Assert.That(recordEntitiesFromDb[1].Hostname, Is.EqualTo(Host2));
            Assert.That(recordEntitiesFromDb[1].StartDate, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[1].EndDate, Is.Null);
            Assert.That(recordEntitiesFromDb[1].LastChecked, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[1].FailureCount, Is.EqualTo(0));
            Assert.That(recordEntitiesFromDb[1].ResultCode, Is.EqualTo(0));
        }

        [Test]
        public async Task CorrectlyUpdatesUnchangedRecords()
        {
            ulong domainId = CreateDomain(Domain);
            ulong recordId1 = CreateMxRecord(Preference1, Host1, domainId, DateTime.UtcNow);
            ulong recordId2 = CreateMxRecord(Preference2, Host2, domainId, DateTime.UtcNow);

            List<RecordEntity> recordEntities = new List<RecordEntity>
            {
                new RecordEntity((int)recordId1, new DomainEntity((int)domainId, Domain), new MxRecordInfo(Host1, Preference1), RCode.NoError, 0),
                new RecordEntity((int)recordId2, new DomainEntity((int)domainId, Domain), new MxRecordInfo(Host2, Preference2), RCode.NoError, 0),
            };

            await _mxRecordDao.InsertOrUpdateRecords(recordEntities);

            List<MxRecord> recordEntitiesFromDb = GetAllRecordEntities();

            Assert.That(recordEntitiesFromDb.Count, Is.EqualTo(2));

            Assert.That(recordEntitiesFromDb[0].DomainId, Is.EqualTo(domainId));
            Assert.That(recordEntitiesFromDb[0].Preference, Is.EqualTo(Preference1));
            Assert.That(recordEntitiesFromDb[0].Hostname, Is.EqualTo(Host1));
            Assert.That(recordEntitiesFromDb[0].StartDate, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[0].EndDate, Is.Null);
            Assert.That(recordEntitiesFromDb[0].LastChecked, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[0].FailureCount, Is.EqualTo(0));
            Assert.That(recordEntitiesFromDb[0].ResultCode, Is.EqualTo(0));

            Assert.That(recordEntitiesFromDb[1].DomainId, Is.EqualTo(domainId));
            Assert.That(recordEntitiesFromDb[1].Preference, Is.EqualTo(Preference2));
            Assert.That(recordEntitiesFromDb[1].Hostname, Is.EqualTo(Host2));
            Assert.That(recordEntitiesFromDb[1].StartDate, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[1].EndDate, Is.Null);
            Assert.That(recordEntitiesFromDb[1].LastChecked, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[1].FailureCount, Is.EqualTo(0));
            Assert.That(recordEntitiesFromDb[1].ResultCode, Is.EqualTo(0));
        }

        [Test]
        public async Task CorrectlyExpiresOldRecords()
        {
            ulong domainId = CreateDomain(Domain);
            ulong recordId1 = CreateMxRecord(Preference1, Host1, domainId, DateTime.UtcNow);
            ulong recordId2 = CreateMxRecord(Preference2, Host2, domainId, DateTime.UtcNow);

            DateTime now = DateTime.Now.Truncate(TimeSpan.FromSeconds(1));
            DateTime nowPlusOne = now.AddSeconds(1);

            List<RecordEntity> recordEntities = new List<RecordEntity>
            {
                new RecordEntity((int)recordId1, new DomainEntity((int)domainId, Domain), new MxRecordInfo(Host1, Preference1), RCode.NoError, 0, now),
                new RecordEntity((int)recordId2, new DomainEntity((int)domainId, Domain), new MxRecordInfo(Host2, Preference2), RCode.NoError, 0, nowPlusOne),
            };

            await _mxRecordDao.InsertOrUpdateRecords(recordEntities);

            List<MxRecord> recordEntitiesFromDb = GetAllRecordEntities();

            Assert.That(recordEntitiesFromDb.Count, Is.EqualTo(2));

            Assert.That(recordEntitiesFromDb[0].DomainId, Is.EqualTo(domainId));
            Assert.That(recordEntitiesFromDb[0].Preference, Is.EqualTo(Preference1));
            Assert.That(recordEntitiesFromDb[0].Hostname, Is.EqualTo(Host1));
            Assert.That(recordEntitiesFromDb[0].StartDate, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[0].EndDate, Is.EqualTo(now));
            Assert.That(recordEntitiesFromDb[0].LastChecked, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[0].FailureCount, Is.EqualTo(0));
            Assert.That(recordEntitiesFromDb[0].ResultCode, Is.EqualTo(0));

            Assert.That(recordEntitiesFromDb[1].DomainId, Is.EqualTo(domainId));
            Assert.That(recordEntitiesFromDb[1].Preference, Is.EqualTo(Preference2));
            Assert.That(recordEntitiesFromDb[1].Hostname, Is.EqualTo(Host2));
            Assert.That(recordEntitiesFromDb[1].StartDate, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[1].EndDate, Is.EqualTo(nowPlusOne));
            Assert.That(recordEntitiesFromDb[1].LastChecked, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[1].FailureCount, Is.EqualTo(0));
            Assert.That(recordEntitiesFromDb[1].ResultCode, Is.EqualTo(0));
        }

        [Test]
        public async Task CorrectlyUpdatesFailureCount()
        {
            ulong domainId = CreateDomain(Domain);
            ulong recordId1 = CreateMxRecord(Preference1, Host1, domainId, DateTime.UtcNow);

            DateTime now = DateTime.Now.Truncate(TimeSpan.FromSeconds(1));

            int failureCount = 1;
            List<RecordEntity> recordEntities = new List<RecordEntity>
            {
                new RecordEntity((int)recordId1, new DomainEntity((int)domainId, Domain), new MxRecordInfo(Host1, Preference1), RCode.NoError, failureCount, now)
            };

            await _mxRecordDao.InsertOrUpdateRecords(recordEntities);

            List<MxRecord> recordEntitiesFromDb = GetAllRecordEntities();

            Assert.That(recordEntitiesFromDb.Count, Is.EqualTo(1));

            Assert.That(recordEntitiesFromDb[0].DomainId, Is.EqualTo(domainId));
            Assert.That(recordEntitiesFromDb[0].Preference, Is.EqualTo(Preference1));
            Assert.That(recordEntitiesFromDb[0].Hostname, Is.EqualTo(Host1));
            Assert.That(recordEntitiesFromDb[0].StartDate, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[0].EndDate, Is.EqualTo(now));
            Assert.That(recordEntitiesFromDb[0].LastChecked, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[0].FailureCount, Is.EqualTo(failureCount));
            Assert.That(recordEntitiesFromDb[0].ResultCode, Is.EqualTo(0));
        }

        [Test]
        public async Task CorrectlyUpdatesErrorStatus()
        {
            ulong domainId = CreateDomain(Domain);
            ulong recordId1 = CreateMxRecord(Preference1, Host1, domainId, DateTime.UtcNow);

            DateTime now = DateTime.Now.Truncate(TimeSpan.FromSeconds(1));

            RCode responseCode = RCode.ServFail;
            List<RecordEntity> recordEntities = new List<RecordEntity>
            {
                new RecordEntity((int)recordId1, new DomainEntity((int)domainId, Domain), new MxRecordInfo(Host1, Preference1), responseCode, 0, now)
            };

            await _mxRecordDao.InsertOrUpdateRecords(recordEntities);

            List<MxRecord> recordEntitiesFromDb = GetAllRecordEntities();

            Assert.That(recordEntitiesFromDb.Count, Is.EqualTo(1));

            Assert.That(recordEntitiesFromDb[0].DomainId, Is.EqualTo(domainId));
            Assert.That(recordEntitiesFromDb[0].Preference, Is.EqualTo(Preference1));
            Assert.That(recordEntitiesFromDb[0].Hostname, Is.EqualTo(Host1));
            Assert.That(recordEntitiesFromDb[0].StartDate, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[0].EndDate, Is.EqualTo(now));
            Assert.That(recordEntitiesFromDb[0].LastChecked, Is.Not.Null);
            Assert.That(recordEntitiesFromDb[0].FailureCount, Is.EqualTo(0));
            Assert.That(recordEntitiesFromDb[0].ResultCode, Is.EqualTo((ushort)responseCode));
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        #region Test Support

        private void SetConfig(IRecordImporterConfig config, int refreshIntervalSeconds, int failureRefreshIntervalSeconds, int dnsRecordLimit)
        {
            A.CallTo(() => config.RefreshIntervalSeconds).Returns(refreshIntervalSeconds);
            A.CallTo(() => config.FailureRefreshIntervalSeconds).Returns(failureRefreshIntervalSeconds);
            A.CallTo(() => config.DnsRecordLimit).Returns(dnsRecordLimit);
        }

        private ulong CreateDomain(string domainName)
        {
            MySqlHelper.ExecuteNonQuery(ConnectionString, $@"INSERT INTO `domain`(`name`, `created_by`) VALUES('{domainName}', 'test');");
            return (ulong)MySqlHelper.ExecuteScalar(ConnectionString, "SELECT LAST_INSERT_ID();");
        }

        private ulong CreateNsRecord(string hostName, ulong domainId, DateTime lastChecked, int failureCount = 0)
        {
            MySqlHelper.ExecuteNonQuery(ConnectionString, $@"INSERT INTO `dns_record_ns`(`domain_id`, `hostname`, `last_checked`, `failure_count`, `result_code`) VALUES({domainId}, '{hostName}', '{lastChecked:yyyy-MM-dd HH:mm:ss}', {failureCount}, 0);");
            return (ulong)MySqlHelper.ExecuteScalar(ConnectionString, "SELECT LAST_INSERT_ID();");
        }

        private ulong CreateMxRecord(int preference, string hostname, ulong domainId, DateTime lastChecked, int failureCount = 0)
        {
            MySqlHelper.ExecuteNonQuery(ConnectionString, $@"INSERT INTO `dns_record_mx`(`domain_id`, `preference`, `hostname`, `last_checked`, `failure_count`, `result_code`) VALUES({domainId}, {preference}, '{hostname}', '{lastChecked:yyyy-MM-dd HH:mm:ss}', {failureCount}, 0);");
            return (ulong)MySqlHelper.ExecuteScalar(ConnectionString, "SELECT LAST_INSERT_ID();");
        }

        private List<MxRecord> GetAllRecordEntities()
        {
            List<MxRecord> records = new List<MxRecord>();
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM dns_record_mx"))
            {
                while (reader.Read())
                {
                    MxRecord recordEntity = new MxRecord(
                        reader.GetInt32("id"),
                        reader.GetInt32("domain_id"),
                        reader.GetInt32("preference"),
                        reader.GetString("hostname"),
                        reader.GetDateTime("start_date"),
                        reader.GetDateTimeNullable("end_date"),
                        reader.GetDateTime("last_checked"),
                        reader.GetInt16("failure_count"),
                        reader.GetInt16("result_code"));

                    records.Add(recordEntity);
                }
            }
            return records;
        }

        private class MxRecord
        {
            public MxRecord(
                int id,
                int domainId,
                int preference,
                string hostname,
                DateTime startDate,
                DateTime? endDate,
                DateTime lastChecked,
                int failureCount,
                int resultCode)
            {
                Id = id;
                DomainId = domainId;
                Preference = preference;
                Hostname = hostname;
                StartDate = startDate;
                EndDate = endDate;
                LastChecked = lastChecked;
                FailureCount = failureCount;
                ResultCode = resultCode;
            }

            public int Id { get; }
            public int DomainId { get; }
            public int Preference { get; }
            public string Hostname { get; }
            public DateTime StartDate { get; }
            public DateTime? EndDate { get; }
            public DateTime LastChecked { get; }
            public int FailureCount { get; }
            public int ResultCode { get; }
        }

        #endregion Test Support
    }
}
