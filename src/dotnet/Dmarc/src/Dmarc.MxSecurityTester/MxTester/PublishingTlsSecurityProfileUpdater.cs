using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;
using Dmarc.MxSecurityTester.Contract.Messages;
using Dmarc.MxSecurityTester.Dao.Entities;

namespace Dmarc.MxSecurityTester.MxTester
{
    internal interface IPublishingTlsSecurityProfileUpdater : ITlsSecurityProfileUpdater
    {
    }

    internal class PublishingTlsSecurityProfileUpdater : IPublishingTlsSecurityProfileUpdater
    {
        private readonly ITlsSecurityProfileUpdater _tlsSecurityProfileUpdater;
        private readonly IPublisher _publisher;
        private readonly ILogger _log;

        public PublishingTlsSecurityProfileUpdater(IPersistentTlsSecurityProfileUpdater tlsSecurityProfileUpdater, 
            IPublisher publisher, 
            ILogger log)
        {
            _tlsSecurityProfileUpdater = tlsSecurityProfileUpdater;
            _publisher = publisher;
            _log = log;
        }

        public async Task<List<DomainTlsSecurityProfile>> UpdateSecurityProfiles(List<DomainTlsSecurityProfile> tlsSecurityProfiles)
        {
            List<DomainTlsSecurityProfile> domainTlsSecurityProfiles = await _tlsSecurityProfileUpdater.UpdateSecurityProfiles(tlsSecurityProfiles);

            foreach (DomainTlsSecurityProfile domainTlsSecurityProfile in domainTlsSecurityProfiles)
            {
                await _publisher.Publish(new DomainTlsProfileChanged(domainTlsSecurityProfile.Domain.Id));
                _log.Debug($"Published DomainTlsProfileChangedMessage for domain: ({domainTlsSecurityProfile.Domain.Id}:{domainTlsSecurityProfile.Domain.Name})");
            }

            return domainTlsSecurityProfiles;
        }
    }
}
