using System.Collections.Generic;
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

        public DnsResolverWrapper(List<IPEndPoint> ipEndPoints)
        {
            _resolver = new Resolver(ipEndPoints.ToArray());
            _resolver.TransportType = TransportType.Tcp;
        }

        public  Task<Response> GetRecord(string domain, QType qType)
        {
            return _resolver.Query(domain, qType);
        }
    }
}