using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.DnsRecord.Importer.Lambda.Dns
{
    public interface IDnsNameServerProvider
    {
        List<IPAddress> GetNameServers();
    }

    public class LinuxDnsNameServerProvider : IDnsNameServerProvider
    {
        private readonly ILogger _log;
        private const string NameServerFileName = "/etc/resolv.conf";
        private static readonly Regex Regex = new Regex(@"(?<=(?:^nameserver\s))(.*)$", RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromSeconds(1));

        public LinuxDnsNameServerProvider(ILogger log)
        {
            _log = log;
        }

        public List<IPAddress> GetNameServers()
        {
            string nameServerFileContents = File.ReadAllText(NameServerFileName);
            MatchCollection matches = Regex.Matches(nameServerFileContents);

            List<string> ipStrings = (from Match match in matches select match.Value).Distinct().ToList();

            List<IPAddress> ipAddresses = new List<IPAddress>();
            foreach (var ipString in ipStrings)
            {
                IPAddress ipAddress;
                if (IPAddress.TryParse(ipString, out ipAddress))
                {
                    ipAddresses.Add(ipAddress);
                }
                else
                {
                    _log.Warn($"Failed to parse {ipString} to IPAddress");
                }
            }
            return ipAddresses;
        }
    }
}
