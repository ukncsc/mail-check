using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.DnsRecord.Evaluator.Seeding.Dao.Entities;

namespace Dmarc.DnsRecord.Evaluator.Seeding.Dao
{
    public class DmarcRecordDao : DnsRecordDao<DmarcRecord>
    {
        public DmarcRecordDao(IConnectionInfo connectionInfo) 
            : base(connectionInfo, DmarcRecordDaoResource.SelectCurrentDmarcRecords)
        {
        }

        protected override async Task<List<DmarcRecord>> CreateRecords(DbDataReader reader)
        {
            List<DmarcRecord> dmarcRecords = new List<DmarcRecord>();
            while (await reader.ReadAsync())
            {
                int domainId = reader.GetInt32("domain_id");
                string domainName = reader.GetString("domain_name");
                string record = reader.GetString("record");

                dmarcRecords.Add(new DmarcRecord(new Domain(domainId, domainName), record));
            }
            return dmarcRecords;
        }
    }
}
