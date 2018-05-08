using Dmarc.DnsRecord.Evaluator.Seeding.Dao.Entities;

namespace Dmarc.DnsRecord.Evaluator.Seeding
{
    public class SpfRecord
    {
        public SpfRecord(Domain domain, string record)
        {
            Domain = domain;
            Record = record;
        }

        public Domain Domain { get; }
        public string Record { get; }
    }
}