using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Dns;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Importer.Lambda.Test.Client
{
    [TestFixture]
    [Category("Integration")]
    public class SpfRecordDnsClientIntegrationTests
    {
        private const string Domain = "<domain_to_test_here>";
        private IDnsNameServerProvider _dnsNameServerProvider;
        private DnsResolverWrapper _dnsResolver;
        private SpfRecordDnsClient _client;

        [SetUp]
        public void SetUp()
        {
            _dnsNameServerProvider = A.Fake<IDnsNameServerProvider>();
            A.CallTo(() => _dnsNameServerProvider.GetNameServers()).Returns(new List<IPAddress>
            {
                IPAddress.Parse("8.8.8.8")
            });
            _dnsResolver = new DnsResolverWrapper(_dnsNameServerProvider);
            _client  = new SpfRecordDnsClient(_dnsResolver, A.Fake<ILogger>());
        }

        [Test]
        public async Task CorrectProvidesSpfRecordWhenRecordSplitOverMultipleStrings()
        {
            DnsResponse dnsResponse = await _client.GetRecord(Domain);
        }
    }
}
