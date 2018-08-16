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
using NUnit.Framework;
using Dmarc.MxSecurityTester.MxTester;
using FakeItEasy;
using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityTester.Test.MxTester
{
    [TestFixture]
    public class PublishingCertSecurityProfileUpdaterTests
    {
        private PublishingCertsSecurityProfileUpdater _publishingCertsSecurityProfileUpdater;
        private IPublishingTlsSecurityProfileUpdater _publishingTlsSecurityProfileUpdater;
        private IPublisher _publisher;

        [SetUp]
        public void SetUp()
        {
            _publishingTlsSecurityProfileUpdater = A.Fake<IPublishingTlsSecurityProfileUpdater>();
            _publisher = A.Fake<IPublisher>();

            _publishingTlsSecurityProfileUpdater = A.Fake<IPublishingTlsSecurityProfileUpdater>();
            _publishingCertsSecurityProfileUpdater = new PublishingCertsSecurityProfileUpdater(
                _publishingTlsSecurityProfileUpdater,
                _publisher,
                A.Fake<ICertificatePublisherConfig>(),
                A.Fake<ILogger>());
        }

        [Test]
        public async Task OnlyCurrentItemsPublished()
        {
            string domain = "abc.com";
            string mailServer = "mail.abc.com";
            List<DomainTlsSecurityProfile> profiles = new List<DomainTlsSecurityProfile>
            {
                new DomainTlsSecurityProfile(new Domain(1, domain),
                    new List<MxRecordTlsSecurityProfile>
                    {
                        new MxRecordTlsSecurityProfile(new MxRecord(1, mailServer), new TlsSecurityProfile(1,
                            DateTime.UtcNow,
                            CreateTlsResults(new List<X509Certificate2> {TestCertificates.Certificate1}))),
                        new MxRecordTlsSecurityProfile(new MxRecord(1, mailServer), new TlsSecurityProfile(1, null,
                            CreateTlsResults(new List<X509Certificate2> {TestCertificates.Certificate2})))
                    })
            };

            List<CertificateResultMessage> results = new List<CertificateResultMessage>();

            A.CallTo(() =>
                    _publishingTlsSecurityProfileUpdater.UpdateSecurityProfiles(A<List<DomainTlsSecurityProfile>>._))
                .Returns(profiles);

            A.CallTo(() => _publisher.Publish(A<CertificateResultMessage>._, A<string>._))
                .Invokes(_ => results.Add((CertificateResultMessage)_.Arguments[0]));

            List<DomainTlsSecurityProfile> processedProfiles =
                await _publishingCertsSecurityProfileUpdater.UpdateSecurityProfiles(profiles);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.First().Domain, Is.EqualTo(domain));
            Assert.That(results.First().Hosts.Count, Is.EqualTo(1));
            Assert.That(results.First().Hosts.First().HostName, Is.EqualTo(mailServer));
            Assert.That(results.First().Hosts.First().Certificates.Count, Is.EqualTo(1));
            Assert.That(results.First().Hosts.First().Certificates.First(),
                Is.EqualTo(Convert.ToBase64String(TestCertificates.Certificate2.RawData)));
            Assert.AreEqual(results.First().Hosts.First().SelectedCipherSuites.First().CipherSuite, "TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA");
        }

        private TlsTestResults CreateTlsResults(List<X509Certificate2> certs)
        {
            return new TlsTestResults(1,
                new TlsTestResultsWithoutCertificate(
                    new TlsTestResult(null, CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA, null, null, null, null, null),
                        null, null, null, null, null, null, null, null, null, null, null),
                    certs);
        }
    }
}
