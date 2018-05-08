using System.Threading.Tasks;

namespace Dmarc.DnsRecord.Importer.Lambda.Dns
{
    public interface IDnsRecordClient
    {
        Task<DnsResponse> GetRecord(string domain);
    }
}