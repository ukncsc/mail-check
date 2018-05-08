using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class ReceivingEncrypted
    {
        public Domain Domain { get; }
        public List<MxRecord> MxRecords { get; }

        public ReceivingEncrypted(Domain domain, List<MxRecord> mxRecords)
        {
            Domain = domain;
            MxRecords = mxRecords;
        }
    }
}