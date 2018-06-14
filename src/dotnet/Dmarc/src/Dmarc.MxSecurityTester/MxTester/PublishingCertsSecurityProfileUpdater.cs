using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Contract.Messages;
using Dmarc.MxSecurityTester.Dao.Entities;
using Newtonsoft.Json;

namespace Dmarc.MxSecurityTester.MxTester
{
    internal interface IPublishingCertsSecurityProfileUpdater : ITlsSecurityProfileUpdater
    {
    }

    internal class PublishingCertsSecurityProfileUpdater : IPublishingCertsSecurityProfileUpdater
    {
        private readonly ITlsSecurityProfileUpdater _tlsSecurityProfileUpdater;
        private readonly IPublisher _publisher;
        private readonly ILogger _log;
        private readonly ICertificatePublisherConfig _config;

        public PublishingCertsSecurityProfileUpdater(IPublishingTlsSecurityProfileUpdater tlsSecurityProfileUpdater,
            IPublisher publisher,
            ICertificatePublisherConfig config,
            ILogger log)
        {
            _tlsSecurityProfileUpdater = tlsSecurityProfileUpdater;
            _publisher = publisher;
            _config = config;
            _log = log;
        }

        public async Task<List<DomainTlsSecurityProfile>> UpdateSecurityProfiles(List<DomainTlsSecurityProfile> tlsSecurityProfiles)
        {
            List<DomainTlsSecurityProfile> domainTlsSecurityProfiles = await _tlsSecurityProfileUpdater.UpdateSecurityProfiles(tlsSecurityProfiles);

            foreach (DomainTlsSecurityProfile domainTlsSecurityProfile in domainTlsSecurityProfiles)
            {
                Dictionary<ulong, HostInfo> hosts = new Dictionary<ulong, HostInfo>();

                foreach (MxRecordTlsSecurityProfile profile in domainTlsSecurityProfile.Profiles)
                {
                    if (!hosts.ContainsKey(profile.MxRecord.Id))
                    {
                        hosts.Add(profile.MxRecord.Id, new HostInfo(profile.MxRecord.Hostname));
                    }

                    hosts[profile.MxRecord.Id].Certificates.AddRange(
                        profile.TlsSecurityProfile.Results.Certificates.Select(x =>
                            Convert.ToBase64String(x.RawData)).Distinct());
                }

                await _publisher.Publish(new CertificateResultMessage(domainTlsSecurityProfile.Domain.Name, hosts.Values.ToList()),
                    _config.PublisherConnectionString);
                _log.Debug(
                    $"Published DomainTlsProfileChangedMessage for domain: ({domainTlsSecurityProfile.Domain.Id}:{domainTlsSecurityProfile.Domain.Name})");
            }

            return domainTlsSecurityProfiles;
        }
    }
}
