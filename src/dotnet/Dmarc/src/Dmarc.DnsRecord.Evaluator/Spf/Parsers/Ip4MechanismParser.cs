using System;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public class Ip4MechanismParser : IMechanismParserStrategy
    {
        private readonly IIp4AddrParser _ip4AddrParser;
        private readonly IIp4CidrBlockParser _cidrBlockParser;

        public Ip4MechanismParser(IIp4AddrParser ip4AddrParser, IIp4CidrBlockParser cidrBlockParser)
        {
            _ip4AddrParser = ip4AddrParser;
            _cidrBlockParser = cidrBlockParser;
        }

        //"ip4" ":" ip4-network [ ip4-cidr-length ]
        public Term Parse(string mechanism, Qualifier qualifier, string arguments)
        {
            string[] splits = arguments.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

            Ip4Addr ipAddress = _ip4AddrParser.Parse(splits.ElementAtOrDefault(0));

            Ip4CidrBlock cidrBlock = _cidrBlockParser.Parse(splits.ElementAtOrDefault(1));

            return new Ip4(mechanism, qualifier, ipAddress, cidrBlock);
        }

        public string Mechanism => "ip4";
    }
}