using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.MxSecurityTester.Dao;
using Dmarc.MxSecurityTester.Dao.Entities;

namespace Dmarc.MxSecurityTester.MxTester
{
    internal interface IMxSecurityTesterProcessor
    {
        Task Process();
    }

    internal class MxSecurityTesterProcessor : IMxSecurityTesterProcessor
    {
        private readonly IDomainTlsSecurityProfileDao _domainTlsSecurityProfileDao;
        private readonly IPublishingCertsSecurityProfileUpdater _securityProfileUpdater;
        private readonly ILogger _log;

        public MxSecurityTesterProcessor(IDomainTlsSecurityProfileDao domainTlsSecurityProfileDao,
            IPublishingCertsSecurityProfileUpdater securityProfileUpdater,
            ILogger log
            )
        {
            _domainTlsSecurityProfileDao = domainTlsSecurityProfileDao;
            _securityProfileUpdater = securityProfileUpdater;
            _log = log;
        }

        public async Task Process()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            //this dao will need updating to read new data structure
            //as will DomainTlsSecurityProfile to have all test results
            List<DomainTlsSecurityProfile> securityProfilesToUpdate = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();
            _log.Debug($"Found {securityProfilesToUpdate.Count} domains to update.");
            int itemsProcessed = 0;
            while (securityProfilesToUpdate.Any())
            {
                await _securityProfileUpdater.UpdateSecurityProfiles(securityProfilesToUpdate);

                itemsProcessed += securityProfilesToUpdate.Count;

                securityProfilesToUpdate = await _domainTlsSecurityProfileDao.GetSecurityProfilesForUpdate();
                _log.Debug($"Found {securityProfilesToUpdate.Count} domains to update.");
            }
            _log.Debug($"Processing {itemsProcessed} domains took: {stopwatch.Elapsed}");
            stopwatch.Stop();
        }
    }
}
