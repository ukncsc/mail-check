using System.Net;
using System.Net.Sockets;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface IIp4AddrParser
    {
        Ip4Addr Parse(string ipAddressString);
    }

    public class Ip4AddrParser : IIp4AddrParser
    {
        public Ip4Addr Parse(string ipAddressString)
        {
            Ip4Addr ip4Addr = new Ip4Addr(ipAddressString);
            IPAddress ipAddress;
            if (IPAddress.TryParse(ipAddressString, out ipAddress))
            {
                if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
                {
                    string errorMessage = string.Format(SpfParserResource.InvalidValueErrorMessage, "ipv4 address", ipAddressString);
                    ip4Addr.AddError(new Error(ErrorType.Error, errorMessage));
                }
            }
            else
            {
                string errorMessage = string.Format(SpfParserResource.InvalidValueErrorMessage, "ip address", ipAddressString);
                ip4Addr.AddError(new Error(ErrorType.Error, errorMessage));
            }

            return ip4Addr;
        }
    }
}