using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Heijden.Dns.Portable;
using Heijden.DNS;

namespace Dmarc.DnsRecord.Importer.Lambda.Dns
{
    public interface IDnsResolver
    {
        Task<Response> GetRecord(string domain, QType qType);
    }

    public class DnsResolverWrapper : IDnsResolver
    {
        private readonly Resolver _resolver;

        public DnsResolverWrapper(IDnsNameServerProvider dnsNameServerProvider)
        {
            _resolver = new Resolver(dnsNameServerProvider.GetNameServers().Select(_ => new IPEndPoint(_, 53)).ToArray());
            _resolver.TransportType = TransportType.Tcp;
        }

        public  Task<Response> GetRecord(string domain, QType qType)
        {
            return _resolver.Query(domain, qType);
        }
    }
}