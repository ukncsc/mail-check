using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface IDualCidrBlockParser
    {
        DualCidrBlock Parse(string ip4CidrBlockString, string ip6CidrBlockString);
    }

    public class DualCidrBlockParser : IDualCidrBlockParser
    {
        private readonly IIp4CidrBlockParser _ip4CidrBlockParser;
        private readonly IIp6CidrBlockParser _ip6CidrBlockParser;

        public DualCidrBlockParser(IIp4CidrBlockParser ip4CidrBlockParser, IIp6CidrBlockParser ip6CidrBlockParser)
        {
            _ip4CidrBlockParser = ip4CidrBlockParser;
            _ip6CidrBlockParser = ip6CidrBlockParser;
        }

        public DualCidrBlock Parse(string ip4CidrBlockString, string ip6CidrBlockString)
        {
            Ip4CidrBlock ip4CidrBlock = _ip4CidrBlockParser.Parse(ip4CidrBlockString);
            Ip6CidrBlock ip6CidrBlock = _ip6CidrBlockParser.Parse(ip6CidrBlockString);

            return new DualCidrBlock(ip4CidrBlock, ip6CidrBlock);
        }
    }
}