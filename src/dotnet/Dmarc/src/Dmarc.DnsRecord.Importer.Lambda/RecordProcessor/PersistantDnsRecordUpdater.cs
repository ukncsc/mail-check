using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.DnsRecord.Importer.Lambda.Dao;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;

namespace Dmarc.DnsRecord.Importer.Lambda.RecordProcessor
{
    public class PersistantDnsRecordUpdater : IDnsRecordUpdater
    {
        private readonly IDnsRecordUpdater _dnsRecordUpdater;
        private readonly IDnsRecordDao _dao;

        public PersistantDnsRecordUpdater(IDnsRecordUpdater dnsRecordUpdater, IDnsRecordDao dao)
        {
            _dnsRecordUpdater = dnsRecordUpdater;
            _dao = dao;
        }

        public async Task<List<RecordEntity>> UpdateRecord(Dictionary<DomainEntity, List<RecordEntity>> records)
        {
            List<RecordEntity> recordEntities = await _dnsRecordUpdater.UpdateRecord(records);
            await _dao.InsertOrUpdateRecords(recordEntities);
            return recordEntities;
        }
    }
}
