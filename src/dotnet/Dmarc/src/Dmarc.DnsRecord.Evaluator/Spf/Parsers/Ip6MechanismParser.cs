using System;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public class Ip6MechanismParser : IMechanismParserStrategy
    {
        private readonly IIp6AddrParser _ip6AddrParser;
        private readonly IIp6CidrBlockParser _cidrBlockParser;

        public Ip6MechanismParser(IIp6AddrParser ip6AddrParser, IIp6CidrBlockParser cidrBlockParser)
        {
            _ip6AddrParser = ip6AddrParser;
            _cidrBlockParser = cidrBlockParser;
        }

        //"ip6" ":" ip6-network [ ip6-cidr-length ]
        public Term Parse(string mechanism, Qualifier qualifier, string arguments)
        {
            string[] splits = arguments.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            Ip6Addr ipAddress = _ip6AddrParser.Parse(splits.ElementAtOrDefault(0));

            Ip6CidrBlock cidrBlock = _cidrBlockParser.Parse(splits.ElementAtOrDefault(1));

            return new Ip6(mechanism, qualifier, ipAddress, cidrBlock);
        }

        public string Mechanism => "ip6";
    }
}