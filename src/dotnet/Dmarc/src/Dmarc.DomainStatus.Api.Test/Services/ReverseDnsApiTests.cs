using Dmarc.DomainStatus.Api.Config;
using Dmarc.DomainStatus.Api.Services;
using Dmarc.DomainStatus.Api.Domain;
using FakeItEasy;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dmarc.DomainStatus.Api.Test.Services
{
    [TestFixture]
    public class ReverseDnsApiTests
    {
        private MockHttpMessageHandler _mockHttp;
        private IReverseDnsApiConfig _config;

        [SetUp]
        public void SetUp()
        {
            _mockHttp = new MockHttpMessageHandler();
            _config = A.Fake<IReverseDnsApiConfig>();

            A.CallTo(() => _config.Endpoint).Returns("https://ncsc.gov.uk");
        }

        [Test]
        public async Task ItShouldReturnTheOriginalExportIfTheHttpRequestFails()
        {
            _mockHttp
                .When("https://ncsc.gov.uk/info")
                .Respond(HttpStatusCode.BadRequest);

            var export = new List<AggregateReportExportItem>();
            var result = await GetResults(_mockHttp.ToHttpClient(), export);

            Assert.That(result, Is.EqualTo(export));
        }

        [Test]
        public async Task ItShouldReturnTheOriginalExportIfTheEndpointUriIsInvalid()
        {
            A.CallTo(() => _config.Endpoint).Returns("flim flam");

            var export = new List<AggregateReportExportItem>();
            var result = await GetResults(_mockHttp.ToHttpClient(), export);

            Assert.That(result, Is.EqualTo(export));
        }

        [Test]
        public async Task ItShouldAddUnknownForIpsWithNoDnsResponses()
        {
            _mockHttp
                .When("https://ncsc.gov.uk/info")
                .Respond("application/json",
                    "[{ 'ipAddress': '192.168.1.1', 'dnsResponses': [] }]");

            var result = await GetResults(_mockHttp.ToHttpClient());

            Assert.That(result[0].Ptr, Is.EqualTo("Unknown"));
        }

        [Test]
        public async Task ItShouldAddUnknownForEmptyDnsResponse()
        {
            _mockHttp
                .When("https://ncsc.gov.uk/info")
                .Respond("application/json", "[]");

            var result = await GetResults(_mockHttp.ToHttpClient());

            Assert.That(result[0].Ptr, Is.EqualTo("Unknown"));
        }

        [Test]
        public async Task ItShouldAddMisMatchForIpsWithNoForwardLookupMatches()
        {
            _mockHttp
                .When("https://ncsc.gov.uk/info")
                .Respond("application/json",
                    "[{ 'ipAddress': '192.168.1.1', 'dnsResponses': [{ 'host': 'abc.com' }], 'forwardLookupMatches': [] }]");

            var result = await GetResults(_mockHttp.ToHttpClient());

            Assert.That(result[0].Ptr, Is.EqualTo("Mismatch"));
        }

        [Test]
        public async Task ItShouldReturnTheForwardLookupMatchesIfTheyArePresent()
        {
            _mockHttp
                .When("https://ncsc.gov.uk/info")
                .Respond("application/json",
                    "[{ 'ipAddress': '192.168.1.1', 'dnsResponses': [{ 'host': 'abc.com' }], 'forwardLookupMatches': ['abc.com'] }]");

            var result = await GetResults(_mockHttp.ToHttpClient());

            Assert.That(result[0].Ptr, Is.EqualTo("abc.com"));
        }

        [Test]
        public async Task ItShouldAddAnOrForMultipleForwardLookupMatches()
        {
            _mockHttp
                .When("https://ncsc.gov.uk/info")
                .Respond("application/json",
                    "[{ 'ipAddress': '192.168.1.1', 'dnsResponses': [{ 'host': 'abc.com' }, { 'host': 'xyz.com' }], 'forwardLookupMatches': ['abc.com', 'xyz.com'] }]");

            var result = await GetResults(_mockHttp.ToHttpClient());

            Assert.That(result[0].Ptr, Is.EqualTo("abc.com or xyz.com"));
        }

        private Task<List<AggregateReportExportItem>> GetResults(HttpClient client, List<AggregateReportExportItem> export = null)
        {
            var sut = new ReverseDnsApi(client, _config, A.Fake<ILogger<ReverseDnsApi>>());
            return sut.AddReverseDnsInfoToExport(
                export ?? new List<AggregateReportExportItem>
                {
                    new AggregateReportExportItem("", "192.168.1.1", "", 0, "", "", "", "", DateTime.Now)
                },
                DateTime.Now);
        }
    }
}
