using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Dmarc.Common.Interface.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Config;
using Dmarc.DnsRecord.Importer.Lambda.Dao;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;

namespace Dmarc.DnsRecord.Importer.Lambda.RecordProcessor
{
    public interface IDnsRecordProcessor
    {
        Task Process(ILambdaContext context);
    }

    public class DnsRecordProcessor : IDnsRecordProcessor
    {
        private readonly IDnsRecordDao _dnsRecordDao;
        private readonly IDnsRecordUpdater _dnsRecordUpdater;
        private readonly IRecordImporterConfig _recordImporterConfig;
        private readonly ILogger _log;

        public DnsRecordProcessor(IDnsRecordDao dnsRecordDao,
            IDnsRecordUpdater dnsRecordUpdater,
            IRecordImporterConfig recordImporterConfig,
            ILogger log)
        {
            _dnsRecordDao = dnsRecordDao;
            _dnsRecordUpdater = dnsRecordUpdater;
            _recordImporterConfig = recordImporterConfig;
            _log = log;

            _log.Debug($"DnsRecordProcessor created with following parameters: {Environment.NewLine}" +
                       $"\tDnsRecordLimit: {_recordImporterConfig.DnsRecordLimit}{Environment.NewLine}" +
                       $"\tRefreshIntervalSeconds: {_recordImporterConfig.RefreshIntervalSeconds}{Environment.NewLine}" +
                       $"\tFailureRefreshIntervalSeconds: {_recordImporterConfig.FailureRefreshIntervalSeconds}{Environment.NewLine}" +
                       $"\tRemainingTimeTheshold: {_recordImporterConfig.RemainingTimeTheshold}");
        }

        public async Task Process(ILambdaContext context)
        {
            //domain to records.
            Dictionary<DomainEntity, List<RecordEntity>> entitiesToUpdate;
            Stopwatch stopwatch = Stopwatch.StartNew();
            do
            {
                entitiesToUpdate = await _dnsRecordDao.GetRecordsForUpdate();
                _log.Debug($"Found {entitiesToUpdate.Count} records to update.");

                if (entitiesToUpdate.Any())
                {
                    await _dnsRecordUpdater.UpdateRecord(entitiesToUpdate);
                    _log.Debug($"Processing {entitiesToUpdate.Count} took: {stopwatch.Elapsed}");
                }
                stopwatch.Restart();
            } while (context.RemainingTime >= _recordImporterConfig.RemainingTimeTheshold && entitiesToUpdate.Any());
            stopwatch.Stop();
        }
    }
}
