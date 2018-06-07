using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;

namespace Dmarc.DnsRecord.Importer.Lambda.RecordProcessor
{
    public interface IDnsRecordUpdater
    {
        Task<List<RecordEntity>> UpdateRecord(Dictionary<DomainEntity, List<RecordEntity>> records);
    }
}