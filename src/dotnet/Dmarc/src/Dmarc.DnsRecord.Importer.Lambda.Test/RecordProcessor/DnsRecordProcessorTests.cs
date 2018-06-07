using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Dmarc.Common.Interface.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Config;
using Dmarc.DnsRecord.Importer.Lambda.Dao;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using Dmarc.DnsRecord.Importer.Lambda.RecordProcessor;
using FakeItEasy;
using Heijden.DNS;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Importer.Lambda.Test.RecordProcessor
{
    [TestFixture]
    public class DnsRecordProcessorTests
    {
        private DnsRecordProcessor _dnsRecordProcessor;
        private ILogger _logger;
        private IDnsRecordDao _dnsRecordDao;
        private IDnsRecordUpdater _dnsRecordUpdater;
        private IRecordImporterConfig _spfRecordImporterConfig;

        [SetUp]
        public void SetUp()
        {
            _logger = A.Fake<ILogger>();
            _dnsRecordDao = A.Fake<IDnsRecordDao>();
            _dnsRecordUpdater = A.Fake<IDnsRecordUpdater>();
            _spfRecordImporterConfig = A.Fake<IRecordImporterConfig>();

            _dnsRecordProcessor = new DnsRecordProcessor(_dnsRecordDao, _dnsRecordUpdater, _spfRecordImporterConfig, _logger);
        }

        [Test]
        public async Task RecordsFoundRecordsUpdated()
        {
            DomainEntity domainEntity = new DomainEntity(1, "");

            Dictionary<DomainEntity, List<RecordEntity>> emptyEntities = new Dictionary<DomainEntity, List<RecordEntity>>();

            Dictionary<DomainEntity, List<RecordEntity>> recordEntities = new Dictionary<DomainEntity, List<RecordEntity>>
            {
                { domainEntity,new List<RecordEntity> {new RecordEntity(1, domainEntity, new SpfRecordInfo(""), RCode.NoError, 0)}}
            };

            Task<Dictionary<DomainEntity, List<RecordEntity>>> listWithEntries = Task.FromResult(recordEntities);

            Task<Dictionary<DomainEntity, List<RecordEntity>>> emptyListEntries = Task.FromResult(emptyEntities);

            A.CallTo(() => _dnsRecordDao.GetRecordsForUpdate())
                .ReturnsNextFromSequence(listWithEntries, emptyListEntries);

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();

            await _dnsRecordProcessor.Process(lambdaContext);

            A.CallTo(() => _dnsRecordDao.GetRecordsForUpdate()).MustHaveHappened(Repeated.Exactly.Twice);
            A.CallTo(() => _dnsRecordUpdater.UpdateRecord(A<Dictionary<DomainEntity, List<RecordEntity>>>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task NoRecordsFoundStopsProcessing()
        {
            Dictionary<DomainEntity, List<RecordEntity>> emptyEntities = new Dictionary<DomainEntity, List<RecordEntity>>();

            Task<Dictionary<DomainEntity, List<RecordEntity>>> emptyListEntries = Task.FromResult(emptyEntities);

            A.CallTo(() => _dnsRecordDao.GetRecordsForUpdate())
                .Returns(emptyListEntries);

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            A.CallTo(() => lambdaContext.RemainingTime).Returns(TimeSpan.FromSeconds(60));

            await _dnsRecordProcessor.Process(lambdaContext);

            A.CallTo(() => _dnsRecordDao.GetRecordsForUpdate()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _dnsRecordUpdater.UpdateRecord(A<Dictionary<DomainEntity, List<RecordEntity>>>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task InsufficientTimeLeftStopProcessing()
        {
            DomainEntity domainEntity = new DomainEntity(1, "");

            Dictionary<DomainEntity, List<RecordEntity>> listWithEntries = new Dictionary<DomainEntity, List<RecordEntity>>
            {
                { domainEntity,new List<RecordEntity> { new RecordEntity(1, domainEntity, new SpfRecordInfo(""),  RCode.NoError, 0) }}
            };

            A.CallTo(() => _dnsRecordDao.GetRecordsForUpdate())
                .ReturnsNextFromSequence(listWithEntries, listWithEntries);

            A.CallTo(() => _spfRecordImporterConfig.RemainingTimeTheshold).Returns(TimeSpan.FromSeconds(2));

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            A.CallTo(() => lambdaContext.RemainingTime).Returns(TimeSpan.FromSeconds(1));

            await _dnsRecordProcessor.Process(lambdaContext);

            A.CallTo(() => _dnsRecordDao.GetRecordsForUpdate()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _dnsRecordUpdater.UpdateRecord(A<Dictionary<DomainEntity, List<RecordEntity>>>._)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
