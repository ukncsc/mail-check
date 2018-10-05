using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using Dmarc.DnsRecord.Importer.Lambda.Dns;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Dmarc.DnsRecord.Importer.Lambda.RecordProcessor;
using FakeItEasy;
using Heijden.DNS;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Importer.Lambda.Test.RecordProcessor
{
    [TestFixture]
    public class DnsRecordUpdaterTests
    {
        private IDnsRecordClient _dnsRecordClient;
        private DnsRecordUpdater _dnsRecordsUpdater;

        [SetUp]
        public void SetUp()
        {
            _dnsRecordClient = A.Fake<IDnsRecordClient>();
            _dnsRecordsUpdater = new DnsRecordUpdater(_dnsRecordClient);
        }

        [Test]
        public async Task OldRecordsCorrectlyUpdated()
        {
            string domain = "a.b.com";
            DomainEntity domainEntity = new DomainEntity(1, domain);
            DmarcRecordInfo existingRecord = new DmarcRecordInfo("existing record", string.Empty, false, false);
            Dictionary<DomainEntity, List<RecordEntity>> records = new Dictionary<DomainEntity, List<RecordEntity>>
            {
                { domainEntity, new List<RecordEntity>{new RecordEntity(1, domainEntity, existingRecord, RCode.NoError, 0)}}
            };

            A.CallTo(() => _dnsRecordClient.GetRecord(domain)).Returns(Task.FromResult(new DnsResponse(new List<RecordInfo> { DmarcRecordInfo.EmptyRecordInfo }, RCode.NoError)));

            List<RecordEntity> updatedRecords = await _dnsRecordsUpdater.UpdateRecord(records);

            Assert.That(updatedRecords.Count, Is.EqualTo(2));
            Assert.That(updatedRecords[0].EndDate, Is.Not.Null);
            Assert.That(updatedRecords[0].RecordInfo, Is.EqualTo(existingRecord));
        }

        [Test]
        public async Task ExistingRecordsRemainUntouched()
        {
            string domain = "a.b.com";
            DomainEntity domainEntity = new DomainEntity(1, domain);
            DmarcRecordInfo existingRecord = new DmarcRecordInfo("record", string.Empty, false, false);
            DmarcRecordInfo incomingRecord = new DmarcRecordInfo("record", string.Empty, false, false);

            Dictionary<DomainEntity, List<RecordEntity>> records = new Dictionary<DomainEntity, List<RecordEntity>>
            {
                { domainEntity, new List<RecordEntity>{new RecordEntity(1, domainEntity, incomingRecord, RCode.NoError, 0)}}
            };

            A.CallTo(() => _dnsRecordClient.GetRecord(domain)).Returns(Task.FromResult(new DnsResponse(new List<RecordInfo> { existingRecord }, RCode.NoError)));

            List<RecordEntity> updatedRecords = await _dnsRecordsUpdater.UpdateRecord(records);

            Assert.That(updatedRecords.Count, Is.EqualTo(1));
            Assert.That(updatedRecords[0].EndDate, Is.Null);
            Assert.That(updatedRecords[0].RecordInfo, Is.EqualTo(existingRecord));
        }

        [Test]
        public async Task NewRecordsCorrectlyCreated()
        {
            string domain = "a.b.com";
            DomainEntity domainEntity = new DomainEntity(1, domain);
            DmarcRecordInfo existingRecord = new DmarcRecordInfo("existing record", string.Empty, false, false);
            DmarcRecordInfo newRecord = new DmarcRecordInfo("new record", string.Empty, false, false);

            Dictionary<DomainEntity, List<RecordEntity>> records = new Dictionary<DomainEntity, List<RecordEntity>>
            {
                { domainEntity, new List<RecordEntity>{new RecordEntity(1, domainEntity,existingRecord, RCode.NoError, 0)}}
            };

            A.CallTo(() => _dnsRecordClient.GetRecord(domain)).Returns(Task.FromResult(new DnsResponse(new List<RecordInfo> { newRecord }, RCode.NoError)));

            List<RecordEntity> updatedRecords = await _dnsRecordsUpdater.UpdateRecord(records);

            Assert.That(updatedRecords.Count, Is.EqualTo(2));
            Assert.That(updatedRecords[1].Id, Is.Null);
            Assert.That(updatedRecords[1].EndDate, Is.Null);
            Assert.That(updatedRecords[1].RecordInfo, Is.EqualTo(newRecord));
        }

        [Test]
        public async Task NewRecordsCorrectlyCreatedWhenNewRecordIsEmpty()
        {
            string domain = "a.b.com";
            DomainEntity domainEntity = new DomainEntity(1, domain);
            DmarcRecordInfo existingRecord = new DmarcRecordInfo("existing record", string.Empty, false, false);

            Dictionary<DomainEntity, List<RecordEntity>> records = new Dictionary<DomainEntity, List<RecordEntity>>
            {
                { domainEntity, new List<RecordEntity>{new RecordEntity(1, domainEntity,existingRecord, RCode.NoError, 0)}}
            };

            A.CallTo(() => _dnsRecordClient.GetRecord(domain)).Returns(Task.FromResult(new DnsResponse(new List<RecordInfo> { DmarcRecordInfo.EmptyRecordInfo }, RCode.NoError)));

            List<RecordEntity> updatedRecords = await _dnsRecordsUpdater.UpdateRecord(records);

            Assert.That(updatedRecords.Count, Is.EqualTo(2));
            Assert.That(updatedRecords[1].EndDate, Is.Null);
            Assert.That(updatedRecords[1].RecordInfo, Is.EqualTo(DmarcRecordInfo.EmptyRecordInfo));
        }

        [Test]
        public async Task ReturnsPlaceholderCorrectly()
        {
            string domain = "a.b.com";
            DomainEntity domainEntity = new DomainEntity(1, domain);

            Dictionary<DomainEntity, List<RecordEntity>> records = new Dictionary<DomainEntity, List<RecordEntity>>
            {
                { domainEntity, new List<RecordEntity>{new RecordEntity(1, domainEntity, null, RCode.ServFail, -1)}}
            };

            A.CallTo(() => _dnsRecordClient.GetRecord(domain)).Returns(Task.FromResult(new DnsResponse(new List<RecordInfo> { DmarcRecordInfo.EmptyRecordInfo }, RCode.ServFail)));

            List<RecordEntity> updatedRecords = await _dnsRecordsUpdater.UpdateRecord(records);

            Assert.That(updatedRecords.Count, Is.EqualTo(1));
            Assert.That(updatedRecords[0].EndDate, Is.Null);
            Assert.That(updatedRecords[0].RecordInfo, Is.EqualTo(null));
            Assert.That(updatedRecords[0].FailureCount, Is.EqualTo(-1));
            Assert.That(updatedRecords[0].ResponseCode, Is.EqualTo(RCode.ServFail));
        }

        [Test]
        public async Task MarksFailedRecordsCorrectlyWhenLessThan3Failures()
        {
            string domain = "a.b.com";
            DomainEntity domainEntity = new DomainEntity(1, domain);
            DmarcRecordInfo existingRecord = new DmarcRecordInfo("existing record", string.Empty, false, false);

            Dictionary<DomainEntity, List<RecordEntity>> records = new Dictionary<DomainEntity, List<RecordEntity>>
            {
                { domainEntity, new List<RecordEntity>{new RecordEntity(1, domainEntity,existingRecord, RCode.NoError, 0)}}
            };

            A.CallTo(() => _dnsRecordClient.GetRecord(domain)).Returns(Task.FromResult(new DnsResponse(new List<RecordInfo> { DmarcRecordInfo.EmptyRecordInfo }, RCode.ServFail)));

            List<RecordEntity> updatedRecords = await _dnsRecordsUpdater.UpdateRecord(records);

            Assert.That(updatedRecords.Count, Is.EqualTo(1));
            Assert.That(updatedRecords[0].EndDate, Is.Null);
            Assert.That(updatedRecords[0].RecordInfo, Is.EqualTo(existingRecord));
            Assert.That(updatedRecords[0].FailureCount, Is.EqualTo(1));
            Assert.That(updatedRecords[0].ResponseCode, Is.EqualTo(RCode.ServFail));
        }

        [Test]
        public async Task MarksFailedRecordsCorrectlyWhen3Failures()
        {
            string domain = "a.b.com";
            DomainEntity domainEntity = new DomainEntity(1, domain);
            DmarcRecordInfo existingRecord = new DmarcRecordInfo("existing record", string.Empty, false, false);

            Dictionary<DomainEntity, List<RecordEntity>> records = new Dictionary<DomainEntity, List<RecordEntity>>
            {
                { domainEntity, new List<RecordEntity>{new RecordEntity(1, domainEntity,existingRecord, RCode.ServFail, 3)}}
            };

            A.CallTo(() => _dnsRecordClient.GetRecord(domain)).Returns(Task.FromResult(new DnsResponse(new List<RecordInfo> { DmarcRecordInfo.EmptyRecordInfo }, RCode.ServFail)));

            List<RecordEntity> updatedRecords = await _dnsRecordsUpdater.UpdateRecord(records);

            Assert.That(updatedRecords.Count, Is.EqualTo(2));
            Assert.That(updatedRecords[0].EndDate, Is.Not.Null);
            Assert.That(updatedRecords[0].RecordInfo, Is.EqualTo(existingRecord));
            Assert.That(updatedRecords[0].FailureCount, Is.EqualTo(3));
            Assert.That(updatedRecords[0].ResponseCode, Is.EqualTo(RCode.ServFail));
            Assert.That(updatedRecords[1].EndDate, Is.Null);
            Assert.That(updatedRecords[1].RecordInfo, Is.EqualTo(null));
            Assert.That(updatedRecords[1].FailureCount, Is.EqualTo(-1));
            Assert.That(updatedRecords[1].ResponseCode, Is.EqualTo(RCode.ServFail));
        }

        [Test]
        public async Task FailureCountClearedOutOnSuccessfulResponse()
        {
            string domain = "a.b.com";
            DomainEntity domainEntity = new DomainEntity(1, domain);
            DmarcRecordInfo existingRecord = new DmarcRecordInfo("existing record", string.Empty, false, false);

            Dictionary<DomainEntity, List<RecordEntity>> records = new Dictionary<DomainEntity, List<RecordEntity>>
            {
                { domainEntity, new List<RecordEntity>{new RecordEntity(1, domainEntity, existingRecord, RCode.ServFail, 1)}}
            };

            A.CallTo(() => _dnsRecordClient.GetRecord(domain)).Returns(Task.FromResult(new DnsResponse(new List<RecordInfo> { DmarcRecordInfo.EmptyRecordInfo }, RCode.NoError)));

            List<RecordEntity> updatedRecords = await _dnsRecordsUpdater.UpdateRecord(records);

            Assert.That(updatedRecords.Count, Is.EqualTo(2));
            Assert.That(updatedRecords[0].FailureCount, Is.EqualTo(1));
            Assert.That(updatedRecords[1].FailureCount, Is.EqualTo(0));
        }

        [Test]
        public async Task HandledEmptyRecordListCorrectly()
        {
            string domain = "a.b.com";
            DomainEntity domainEntity = new DomainEntity(1, domain);

            Dictionary<DomainEntity, List<RecordEntity>> records = new Dictionary<DomainEntity, List<RecordEntity>>
            {
                { domainEntity, new List<RecordEntity>()}
            };

            A.CallTo(() => _dnsRecordClient.GetRecord(domain)).Returns(Task.FromResult(new DnsResponse(new List<RecordInfo> { DmarcRecordInfo.EmptyRecordInfo }, RCode.BADALG)));

            List<RecordEntity> updatedRecords = await _dnsRecordsUpdater.UpdateRecord(records);

            Assert.That(updatedRecords.Count, Is.EqualTo(1));
        }
    }
}
