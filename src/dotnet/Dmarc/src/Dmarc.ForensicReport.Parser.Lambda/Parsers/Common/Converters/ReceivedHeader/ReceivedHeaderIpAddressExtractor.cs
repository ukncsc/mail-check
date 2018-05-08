using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters.ReceivedHeader
{
    public interface IReceivedHeaderIpAddressExtractor
    {
        List<IPAddress> ExtractIpAddresses(string input);
    }

    public class ReceivedHeaderIpAddressExtractor : IReceivedHeaderIpAddressExtractor
    {
        private readonly ILogger _log;
        private static readonly Regex Regex = CreateRegEx();

        public ReceivedHeaderIpAddressExtractor(ILogger log)
        {
            _log = log;
        }

        private static Regex CreateRegEx()
        {
            const string ipv4 = @"(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])(?:\.(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])){3}"; //credited to https://github.com/sindresorhus/ip-regex/blob/master/index.js

            const string ipv6 = "\\[(" + // credited to https://gist.github.com/HenkPoley/8899766
                                "([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|" +               // 1:2:3:4:5:6:7:8
                                "([0-9a-fA-F]{1,4}:){1,7}:|" +                              // 1::                              1:2:3:4:5:6:7::
                                "([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|" +              // 1::8             1:2:3:4:5:6::8  1:2:3:4:5:6::8
                                "([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|" +       // 1::7:8           1:2:3:4:5::7:8  1:2:3:4:5::8
                                "([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|" +       // 1::6:7:8         1:2:3:4::6:7:8  1:2:3:4::8
                                "([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|" +       // 1::5:6:7:8       1:2:3::5:6:7:8  1:2:3::8
                                "([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|" +       // 1::4:5:6:7:8     1:2::4:5:6:7:8  1:2::8
                                "[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|" +            // 1::3:4:5:6:7:8   1::3:4:5:6:7:8  1::8  
                                ":((:[0-9a-fA-F]{1,4}){1,7}|:)|" +                          // ::2:3:4:5:6:7:8  ::2:3:4:5:6:7:8 ::8       ::  
                                "fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|" +          // fe80::7:8%eth0   fe80::7:8%1     (link-local IPv6 addresses with zone index)
                                "::(ffff(:0{1,4}){0,1}:){0,1}" +
                                "((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}" +
                                "(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|" +               // ::255.255.255.255   ::ffff:255.255.255.255  ::ffff:0:255.255.255.255  (IPv4-mapped IPv6 addresses and IPv4-translated addresses)
                                "([0-9a-fA-F]{1,4}:){1,4}:" +
                                "((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}" +
                                "(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])" +                // 2001:db8:3:4::192.0.2.33  64:ff9b::192.0.2.33 (IPv4-Embedded IPv6 Address)
                                ")\\]";

            string ipPattern = $@"(?<=(\[|\())(?:{ipv4}|{ipv6})(?=(\]|\)))";

            const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline;

            return new Regex(ipPattern, options, TimeSpan.FromSeconds(1));
        }

        public List<IPAddress> ExtractIpAddresses(string input)
        {
            try
            {
                return (from Match match in Regex.Matches(input) select match.Value).Distinct()
                    .Select(IPAddress.Parse)
                    .ToList();
            }
            catch (Exception)
            {
                _log.Warn("Failed to extract ip addresses.");
                return new List<IPAddress>();
            }
        }
    }
}