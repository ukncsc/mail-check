using System.Collections.Generic;
using Dmarc.DnsRecord.Contract.Messages;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Dmarc.DnsRecord.Importer.Lambda.Mapping;
using Heijden.DNS;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Importer.Lambda.Test.Mapping
{
    [TestFixture]
    public class SpfConfigsUpdatedMapperTests
    {
        private const int Domain1Id = 1;
        private const int Domain2Id = 2;
        private const int Domain3Id = 3;
        private const string Domain1Name = "Domain1Name";
        private const string Domain2Name = "Domain2Name";
        private const string Domain3Name = "Domain3Name";
        private const string Record1 = "Record1";
        private const string Record2 = "Record2";
        private const string Record3 = "Record3";

        private SpfConfigsUpdatedMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _mapper = new SpfConfigsUpdatedMapper();
        }

        [Test]
        public void RecordEntitiesAreCorrectlyMapped()
        {
            List<RecordEntity> entities = new List<RecordEntity>
            {
                new RecordEntity(null, new DomainEntity(Domain1Id, Domain1Name), new SpfRecordInfo(Record1), RCode.NoError, 0),
                new RecordEntity(null, new DomainEntity(Domain1Id, Domain1Name), new SpfRecordInfo(Record2), RCode.NoError, 0),
                new RecordEntity(null, new DomainEntity(Domain2Id, Domain2Name), new SpfRecordInfo(Record3), RCode.NoError, 0)
            };

            SpfConfigsUpdated configs = _mapper.Map(entities);

            Assert.That(configs.SpfConfigs.Count, Is.EqualTo(2));

            Assert.That(configs.SpfConfigs[0].Domain.Id, Is.EqualTo(Domain1Id));
            Assert.That(configs.SpfConfigs[0].Domain.Name, Is.EqualTo(Domain1Name));
            Assert.That(configs.SpfConfigs[0].Records.Count, Is.EqualTo(2));
            Assert.That(configs.SpfConfigs[0].Records[0], Is.EqualTo(Record1));
            Assert.That(configs.SpfConfigs[0].Records[1], Is.EqualTo(Record2));

            Assert.That(configs.SpfConfigs[1].Domain.Id, Is.EqualTo(Domain2Id));
            Assert.That(configs.SpfConfigs[1].Domain.Name, Is.EqualTo(Domain2Name));
            Assert.That(configs.SpfConfigs[1].Records.Count, Is.EqualTo(1));
            Assert.That(configs.SpfConfigs[1].Records[0], Is.EqualTo(Record3));
        }

        [Test]
        public void NullRecordsResultInEmptyList()
        {
            List<RecordEntity> entities = new List<RecordEntity>
            {
                new RecordEntity(null, new DomainEntity(Domain1Id, Domain1Name), new SpfRecordInfo(null), RCode.NoError, 0),
            };

            SpfConfigsUpdated configs = _mapper.Map(entities);

            Assert.That(configs.SpfConfigs.Count, Is.EqualTo(1));

            Assert.That(configs.SpfConfigs[0].Domain.Id, Is.EqualTo(Domain1Id));
            Assert.That(configs.SpfConfigs[0].Domain.Name, Is.EqualTo(Domain1Name));
            Assert.That(configs.SpfConfigs[0].Records.Count, Is.EqualTo(0));
        }

        [Test]
        public void NewAndOldRecordsMapped()
        {
            List<RecordEntity> entities = new List<RecordEntity>
            {
                new RecordEntity(1, new DomainEntity(Domain1Id, Domain1Name), new SpfRecordInfo(Record1), RCode.NoError, 0),
                new RecordEntity(null, new DomainEntity(Domain2Id, Domain2Name), new SpfRecordInfo(Record2), RCode.NoError, 0),
                new RecordEntity(3, new DomainEntity(Domain3Id, Domain3Name), new SpfRecordInfo(Record3), RCode.NoError, 0)
            };

            SpfConfigsUpdated configs = _mapper.Map(entities);

            Assert.That(configs.SpfConfigs.Count, Is.EqualTo(3));
        }
    }
}
