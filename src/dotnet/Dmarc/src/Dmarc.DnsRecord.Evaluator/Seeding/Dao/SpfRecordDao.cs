using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.DnsRecord.Evaluator.Seeding.Dao.Entities;

namespace Dmarc.DnsRecord.Evaluator.Seeding.Dao
{
    public class SpfRecordDao : DnsRecordDao<SpfRecord>
    {
        public SpfRecordDao(IConnectionInfo connectionInfo) 
            : base(connectionInfo, SpfRecordDaoResource.SelectCurrentSpfRecords)
        {
        }

        protected override async Task<List<SpfRecord>> CreateRecords(DbDataReader reader)
        {
            List<SpfRecord> spfRecords = new List<SpfRecord>();
            while (await reader.ReadAsync())
            {
                int domainId = reader.GetInt32("domain_id");
                string domainName = reader.GetString("domain_name");
                string record = reader.GetString("record");

                spfRecords.Add(new SpfRecord(new Domain(domainId, domainName), record));
            }
            return spfRecords;
        }
    }
}