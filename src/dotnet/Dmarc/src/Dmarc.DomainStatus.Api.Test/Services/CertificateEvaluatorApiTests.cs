using Dmarc.DomainStatus.Api.Config;
using Dmarc.DomainStatus.Api.Domain;
using Dmarc.DomainStatus.Api.Services;
using FakeItEasy;
using Flurl.Http.Testing;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dmarc.DomainStatus.Api.Test.Services
{
    [TestFixture]
    public class CertificateEvaluatorApiTests
    {
        private IDomainStatusApiConfig _config;
        private ICertificateEvaluatorApi sut;
        private HttpTest _httpTest;

        [SetUp]
        public void SetUp()
        {
            _config = A.Fake<IDomainStatusApiConfig>();
            sut = new CertificateEvaluatorApiClient(_config, A.Fake<ILogger<CertificateEvaluatorApiClient>>());

            _httpTest = new HttpTest();

            A.CallTo(() => _config.CertificateEvaluatorApiEndpoint).Returns("https://ncsc.gov.uk");
        }

        [TearDown]
        public void TearDown()
        {
            _httpTest.Dispose();
        }

        [Test]
        public async Task ItShouldReturnTheOriginalInfoIfTheHttpRequestFails()
        {
            _httpTest
                .RespondWith("", 403);

            List<DomainSecurityInfo> info = new List<DomainSecurityInfo>();
            List<DomainSecurityInfo> result = await GetResults(info);

            Assert.That(result, Is.EqualTo(info));
        }

        [Test]
        public async Task ItShouldKeepTheExistingSeverityIfItIsHigher()
        {
            _httpTest
                .RespondWithJson(new[] { new { DomainName = "ncsc.gov.uk", HostResults = new[] { new { HostName = "mail.ncsc.gov.uk", Errors = new[] { new { ErrorType = "Inconclusive", Message = "blah" } } } } } });

            List<DomainSecurityInfo> info = info = new List<DomainSecurityInfo>() { new DomainSecurityInfo(new Domain.Domain(1, "ncsc.gov.uk"), true, Status.Error, Status.None, Status.None) };
            List<DomainSecurityInfo> result = await GetResults(info);

            Assert.That(result[0].TlsStatus, Is.EqualTo(Status.Error));
        }

        [Test]
        public async Task ItShouldChangeTheSeverityIfTheCertificateStatusIsHigher()
        {
            _httpTest
                .RespondWithJson(new[] { new { DomainName = "ncsc.gov.uk", HostResults = new[] { new { HostName = "mail.ncsc.gov.uk", Errors = new[] { new { ErrorType = "Warning", Message = "blah" } } } } } });

            List<DomainSecurityInfo> result = await GetResults();

            Assert.That(result[0].TlsStatus, Is.EqualTo(Status.Warning));
        }

        [Test]
        public async Task ItShouldBeASuccessIfThereAreNoErrors()
        {
            _httpTest
                .RespondWithJson(new[] { new { DomainName = "ncsc.gov.uk", HostResults = new[] { new { HostName = "mail.ncsc.gov.uk", Errors = new List<string>() } } } });

            List<DomainSecurityInfo> result = await GetResults();

            Assert.That(result[0].TlsStatus, Is.EqualTo(Status.Success));
        }

        [Test]
        public async Task ItShouldNotChangeTheSeverityForNullHostname()
        {
            _httpTest
                .RespondWithJson(new[] { new { DomainName = "ncsc.gov.uk", HostResults = new[] { new { HostName = null as string, Errors = new[] { new { ErrorType = "Warning", Message = "blah" } } } } } });

            List<DomainSecurityInfo> result = await GetResults();

            Assert.That(result[0].TlsStatus, Is.EqualTo(Status.Success));
        }

        [Test]
        public async Task ItShouldRemainTheSameIfThereAreNoCertificateResults()
        {
            _httpTest
                .RespondWithJson(new List<string>());

            List<DomainSecurityInfo> info = new List<DomainSecurityInfo>()
            {
                new DomainSecurityInfo(new Domain.Domain(1, "ncsc.gov.uk"), true, Status.Success, Status.None, Status.None)
            };

            List<DomainSecurityInfo> result = await GetResults(info);

            Assert.That(result[0], Is.EqualTo(info[0]));
        }

        private Task<List<DomainSecurityInfo>> GetResults(List<DomainSecurityInfo> info = null) =>
            sut.UpdateTlsWithCertificateEvaluatorStatus(
                info ?? new List<DomainSecurityInfo>()
                {
                    new DomainSecurityInfo(new Domain.Domain(1, "ncsc.gov.uk"), true, Status.Success, Status.None, Status.None)
                });
    }
}
