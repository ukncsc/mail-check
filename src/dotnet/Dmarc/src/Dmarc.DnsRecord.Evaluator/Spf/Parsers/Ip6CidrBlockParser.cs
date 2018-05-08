using System.Text.RegularExpressions;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface IIp6CidrBlockParser
    {
        Ip6CidrBlock Parse(string cidrBlock);
    }

    public class Ip6CidrBlockParser : IIp6CidrBlockParser
    {
        private readonly Regex _regex = new Regex("^(0|[1-9]{1}|[1-9]{1}[0-9]{1}|[1]{1}[0-2]{1}[0-8]{1})$");
        private const int DefaultIp6CidrBlock = 128;

        public Ip6CidrBlock Parse(string cidrBlock)
        {
            if (string.IsNullOrEmpty(cidrBlock))
            {
                return new Ip6CidrBlock(DefaultIp6CidrBlock);
            }

            if (_regex.IsMatch(cidrBlock))
            {
                return new Ip6CidrBlock(int.Parse(cidrBlock));
            }

            Ip6CidrBlock ip6CidrBlock = new Ip6CidrBlock(null);

            string errorMessage = string.Format(SpfParserResource.InvalidValueErrorMessage, "ipv6 cidr block", $"{cidrBlock}. Value must be in the range 0-128");
            ip6CidrBlock.AddError(new Error(ErrorType.Error, errorMessage));

            return ip6CidrBlock;
        }
    }
}