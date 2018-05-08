using Dmarc.DnsRecord.Evaluator.Seeding.Dao.Entities;

namespace Dmarc.DnsRecord.Evaluator.Seeding
{
    public class DmarcRecord
    {
        public Domain Domain { get; }
        public string Record { get; }

        public DmarcRecord(Domain domain, string record)
        {
            Domain = domain;
            Record = record;
        }
    }
}