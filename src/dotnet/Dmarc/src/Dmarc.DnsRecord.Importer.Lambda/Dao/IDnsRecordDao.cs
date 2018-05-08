using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;

namespace Dmarc.DnsRecord.Importer.Lambda.Dao
{
    public interface IDnsRecordDao
    {
        Task<Dictionary<DomainEntity,List<RecordEntity>>> GetRecordsForUpdate();

        Task InsertOrUpdateRecords(List<RecordEntity> records);
    }
}