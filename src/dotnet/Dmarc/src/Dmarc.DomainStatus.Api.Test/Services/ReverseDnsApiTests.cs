using Dmarc.DomainStatus.Api.Config;
using Dmarc.DomainStatus.Api.Services;
using Dmarc.DomainStatus.Api.Domain;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Flurl.Http.Testing;

namespace Dmarc.DomainStatus.Api.Test.Services
{
    [TestFixture]
    public class ReverseDnsApiTests
    {
        private IDomainStatusApiConfig _config;
        private IReverseDnsApi sut;
        private HttpTest _httpTest;

        [SetUp]
        public void SetUp()
        {
            _config = A.Fake<IDomainStatusApiConfig>();
            sut = new ReverseDnsApiClient(_config, A.Fake<ILogger<ReverseDnsApiClient>>());

            _httpTest = new HttpTest();

            A.CallTo(() => _config.ReverseDnsApiEndpoint).Returns("https://ncsc.gov.uk");
        }

        [TearDown]
        public void TearDown()
        {
            _httpTest.Dispose();
        }

        [Test]
        public async Task ItShouldReturnTheOriginalExportIfTheHttpRequestFails()
        {
            _httpTest
                .RespondWith("{}", 403);

            var export = new List<AggregateReportExportItem>();
            var result = await GetResults(export);

            Assert.That(result, Is.EqualTo(export));
        }

        [Test]
        public async Task ItShouldReturnTheOriginalExportIfTheEndpointUriIsInvalid()
        {
            A.CallTo(() => _config.ReverseDnsApiEndpoint).Returns("flim flam");

            var export = new List<AggregateReportExportItem>();
            var result = await GetResults(export);

            Assert.That(result, Is.EqualTo(export));
        }

        [Test]
        public async Task ItShouldAddUnknownForIpsWithNoDnsResponses()
        {
            _httpTest
                .RespondWithJson(new[] { new { IpAddress = "192.168.1.1", DnsResponses = new List<string>() } });

            var result = await GetResults();

            Assert.That(result[0].Ptr, Is.EqualTo("Unknown"));
        }

        [Test]
        public async Task ItShouldAddUnknownForEmptyDnsResponse()
        {
            _httpTest
                .RespondWithJson(new List<string>());

            var result = await GetResults();

            Assert.That(result[0].Ptr, Is.EqualTo("Unknown"));
        }

        [Test]
        public async Task ItShouldAddMisMatchForIpsWithNoForwardLookupMatches()
        {
            _httpTest
                .RespondWithJson(new[] { new { IpAddress = "192.168.1.1", DnsResponses = new[] { new { Host = "abc.com" } }, ForwardLookupMatches = new List<string>() } });

            var result = await GetResults();

            Assert.That(result[0].Ptr, Is.EqualTo("Mismatch"));
        }

        [Test]
        public async Task ItShouldReturnTheForwardLookupMatchesIfTheyArePresent()
        {
            _httpTest
                .RespondWithJson(new[] { new { IpAddress = "192.168.1.1", DnsResponses = new[] { new { Host = "abc.com" } }, ForwardLookupMatches = new[] { "abc.com" } } });

            var result = await GetResults();

            Assert.That(result[0].Ptr, Is.EqualTo("abc.com"));
        }

        [Test]
        public async Task ItShouldAddAnOrForMultipleForwardLookupMatches()
        {
            _httpTest
                .RespondWithJson(new[] { new { IpAddress = "192.168.1.1", DnsResponses = new[] { new { Host = "abc.com" }, new { Host = "xyz.com" } }, ForwardLookupMatches = new[] { "abc.com", "xyz.com" } } });

            var result = await GetResults();

            Assert.That(result[0].Ptr, Is.EqualTo("abc.com or xyz.com"));
        }

        private Task<List<AggregateReportExportItem>> GetResults(List<AggregateReportExportItem> export = null) =>
            sut.AddReverseDnsInfoToExport(
                export ?? new List<AggregateReportExportItem>
                {
                    new AggregateReportExportItem("", "192.168.1.1", "", 0, "", "", "", "", DateTime.Now)
                },
                DateTime.Now);
    }
}
