using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class DomainRecords
    {
        public DomainRecords(Domain domain,
            List<MxRecord> mxRecords,
            List<SpfRecord> spfRecords,
            List<DmarcRecord> dmarcRecords)
        {
            Domain = domain;
            MxRecords = mxRecords;
            SpfRecords = spfRecords;
            DmarcRecords = dmarcRecords;
        }

        public Domain Domain { get; }
        public List<MxRecord> MxRecords { get; }
        public List<SpfRecord> SpfRecords { get; }
        public List<DmarcRecord> DmarcRecords { get; }
    }
}
