using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class DomainStatus
    {
        public DomainStatus(Domain domain,
            bool hasNsRecords,
            List<MxRecord> mxRecords, 
            List<SpfRecord> spfRecords, 
            List<DmarcRecord> dmarcRecords)
        {
            Domain = domain;
            HasNsRecords = hasNsRecords;
            MxRecords = mxRecords;
            SpfRecords = spfRecords;
            DmarcRecords = dmarcRecords;
        }

        public Domain Domain { get; }
        public bool HasNsRecords { get; }
        public List<MxRecord> MxRecords { get; }
        public List<SpfRecord> SpfRecords { get; }
        public List<DmarcRecord> DmarcRecords { get; }

    }
}
