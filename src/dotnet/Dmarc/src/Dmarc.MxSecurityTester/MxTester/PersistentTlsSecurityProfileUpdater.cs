using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.MxSecurityTester.Dao;
using Dmarc.MxSecurityTester.Dao.Entities;

namespace Dmarc.MxSecurityTester.MxTester
{
    internal interface IPersistentTlsSecurityProfileUpdater : ITlsSecurityProfileUpdater { }

    internal class PersistentTlsSecurityProfileUpdater : IPersistentTlsSecurityProfileUpdater
    {
        private readonly ITlsSecurityProfileUpdater _tlsSecurityProfileUpdater;
        private readonly IDomainTlsSecurityProfileDao _domainTlsSecurityProfileDao;

        public PersistentTlsSecurityProfileUpdater(ITlsSecurityProfileUpdater tlsSecurityProfileUpdater,
            IDomainTlsSecurityProfileDao domainTlsSecurityProfileDao)
        {
            _tlsSecurityProfileUpdater = tlsSecurityProfileUpdater;
            _domainTlsSecurityProfileDao = domainTlsSecurityProfileDao;
        }

        public async Task<List<DomainTlsSecurityProfile>> UpdateSecurityProfiles(List<DomainTlsSecurityProfile> tlsSecurityProfiles)
        {
            List<DomainTlsSecurityProfile> domainTlsSecurityProfiles = await _tlsSecurityProfileUpdater.UpdateSecurityProfiles(tlsSecurityProfiles);

            await _domainTlsSecurityProfileDao.InsertOrUpdateSecurityProfiles(domainTlsSecurityProfiles);

            return domainTlsSecurityProfiles;
        }
    }
}