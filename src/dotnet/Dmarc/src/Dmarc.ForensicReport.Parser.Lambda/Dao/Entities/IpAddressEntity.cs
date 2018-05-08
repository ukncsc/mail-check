using System.Net;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class IpAddressEntity
    {
        public IpAddressEntity(string ip, string binaryIp)
        {
            Ip = ip;
            BinaryIp = binaryIp;
        }

        public long Id { get; set; }
        public string Ip { get; }
        public string BinaryIp { get; }
    }
}
