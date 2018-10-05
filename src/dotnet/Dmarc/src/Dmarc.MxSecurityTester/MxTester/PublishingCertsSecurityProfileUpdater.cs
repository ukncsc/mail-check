using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Contract.Messages;
using Dmarc.MxSecurityTester.Dao.Entities;
using Dmarc.MxSecurityTester.Util;

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
                List<MxRecordTlsSecurityProfile> currentProfiles = domainTlsSecurityProfile.Profiles
                    .Where(_ => !_.TlsSecurityProfile.EndDate.HasValue).ToList();

                CertificateResultMessage certificateResultMessage = new CertificateResultMessage(
                    domainTlsSecurityProfile.Domain.Name,
                    currentProfiles
                        .Select(_ => new HostInfo(_.MxRecord.Hostname, CheckHostNotFound(_.TlsSecurityProfile.TlsResults.Results),
                            _.TlsSecurityProfile.TlsResults.Certificates.Select(c => Convert.ToBase64String(c.RawData)).ToList(),
                            GetTlsCipherSuitesFromResults(_.TlsSecurityProfile.TlsResults.Results)))
                        .ToList());

                await _publisher.Publish(certificateResultMessage, _config.PublisherConnectionString);

                _log.Debug($"Published DomainTlsProfileChangedMessage for domain: ({domainTlsSecurityProfile.Domain.Id}:{domainTlsSecurityProfile.Domain.Name})");
            }

            return domainTlsSecurityProfiles;
        }

        private bool CheckHostNotFound(TlsTestResultsWithoutCertificate results)
        {
            List<TlsTestResult> tlsTestResults = new List<TlsTestResult>
            {
                results.Tls12AvailableWithBestCipherSuiteSelected,
                results.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
                results.Tls12AvailableWithSha2HashFunctionSelected,
                results.Tls12AvailableWithWeakCipherSuiteNotSelected,
                results.Tls11AvailableWithBestCipherSuiteSelected,
                results.Tls11AvailableWithWeakCipherSuiteNotSelected,
                results.Tls10AvailableWithBestCipherSuiteSelected,
                results.Tls10AvailableWithWeakCipherSuiteNotSelected,
                results.Ssl3FailsWithBadCipherSuite,
                results.TlsSecureEllipticCurveSelected,
                results.TlsSecureDiffieHellmanGroupSelected,
                results.TlsWeakCipherSuitesRejected
            };

            return tlsTestResults.All(_ => _.Error == Error.HOST_NOT_FOUND);
        }

        private List<SelectedCipherSuite> GetTlsCipherSuitesFromResults(TlsTestResultsWithoutCertificate tlsResults) =>
            new List<SelectedCipherSuite>
            {
                new SelectedCipherSuite(TlsTestType.Tls12AvailableWithBestCipherSuiteSelected.ToString(), tlsResults.Tls12AvailableWithBestCipherSuiteSelected?.CipherSuite?.ToString()),
                new SelectedCipherSuite(TlsTestType.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList.ToString(), tlsResults.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList?.CipherSuite?.ToString()),
                new SelectedCipherSuite(TlsTestType.Tls12AvailableWithSha2HashFunctionSelected.ToString(), tlsResults.Tls12AvailableWithSha2HashFunctionSelected?.CipherSuite?.ToString()),
                new SelectedCipherSuite(TlsTestType.Tls12AvailableWithWeakCipherSuiteNotSelected.ToString(), tlsResults.Tls12AvailableWithWeakCipherSuiteNotSelected?.CipherSuite?.ToString()),
                new SelectedCipherSuite(TlsTestType.Tls11AvailableWithBestCipherSuiteSelected.ToString(), tlsResults.Tls11AvailableWithBestCipherSuiteSelected?.CipherSuite?.ToString()),
                new SelectedCipherSuite(TlsTestType.Tls11AvailableWithWeakCipherSuiteNotSelected.ToString(), tlsResults.Tls11AvailableWithWeakCipherSuiteNotSelected?.CipherSuite?.ToString()),
                new SelectedCipherSuite(TlsTestType.Tls10AvailableWithBestCipherSuiteSelected.ToString(), tlsResults.Tls10AvailableWithBestCipherSuiteSelected?.CipherSuite?.ToString()),
                new SelectedCipherSuite(TlsTestType.Tls10AvailableWithWeakCipherSuiteNotSelected.ToString(), tlsResults.Tls10AvailableWithWeakCipherSuiteNotSelected?.CipherSuite?.ToString()),
                new SelectedCipherSuite(TlsTestType.Ssl3FailsWithBadCipherSuite.ToString(), tlsResults.Ssl3FailsWithBadCipherSuite?.CipherSuite?.ToString()),
                new SelectedCipherSuite(TlsTestType.TlsSecureEllipticCurveSelected.ToString(), tlsResults.TlsSecureEllipticCurveSelected?.CipherSuite?.ToString()),
                new SelectedCipherSuite(TlsTestType.TlsSecureDiffieHellmanGroupSelected.ToString(), tlsResults.TlsSecureDiffieHellmanGroupSelected?.CipherSuite?.ToString()),
                new SelectedCipherSuite(TlsTestType.TlsWeakCipherSuitesRejected.ToString(), tlsResults.TlsWeakCipherSuitesRejected?.CipherSuite?.ToString())
            };
    }
}
