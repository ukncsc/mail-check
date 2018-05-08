using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;

namespace Dmarc.DnsRecord.Importer.Lambda.Publisher
{
    public interface IRecordEntityPublisher
    {
        Task Publish(List<RecordEntity> recordEntities);
    }
}