using System.Collections.Generic;

namespace Dmarc.MxSecurityTester.Dao.Entities
{
    public class DomainTlsSecurityProfile
    {
        public DomainTlsSecurityProfile(Domain domain, List<MxRecordTlsSecurityProfile> profiles)
        {
            Domain = domain;
            Profiles = profiles;
        }

        public Domain Domain { get; }

        public List<MxRecordTlsSecurityProfile> Profiles { get; }
    }
}