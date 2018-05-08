using System.Net;
using System.Net.Sockets;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface IIp6AddrParser
    {
        Ip6Addr Parse(string ipAddressString);
    }

    public class Ip6AddrParser : IIp6AddrParser
    {
        public Ip6Addr Parse(string ipAddressString)
        {
            Ip6Addr ip6Addr = new Ip6Addr(ipAddressString);
            IPAddress ipAddress;
            if (IPAddress.TryParse(ipAddressString, out ipAddress))
            {
                if (ipAddress.AddressFamily != AddressFamily.InterNetworkV6)
                {
                    string errorMessage = string.Format(SpfParserResource.InvalidValueErrorMessage, "ipv6 address", ipAddressString);
                    ip6Addr.AddError(new Error(ErrorType.Error, errorMessage));
                }
            }
            else
            {
                string errorMessage = string.Format(SpfParserResource.InvalidValueErrorMessage, "ip address", ipAddressString);
                ip6Addr.AddError(new Error(ErrorType.Error, errorMessage));
            }
            return ip6Addr;
        }
    }
}