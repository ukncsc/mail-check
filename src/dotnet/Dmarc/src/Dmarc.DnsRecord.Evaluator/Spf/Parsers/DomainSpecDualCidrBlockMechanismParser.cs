using System;
using System.Linq;
using System.Text.RegularExpressions;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface IDomainSpecDualCidrBlockMechanismParser
    {
        Term Parse(string mechanism, Qualifier qualifier, string arguments, Func<string, Qualifier, DomainSpec, DualCidrBlock, Term> termFactory);
    }

    public class DomainSpecDualCidrBlockMechanismParser : IDomainSpecDualCidrBlockMechanismParser
    {
        private readonly Regex _regex = new Regex(@"^(?<domain_spec>[^\d\/]{1}[^\/]*)(\/(?<ip4_cidr>[\d]*[^\/\/])){0,1}(\/\/(?<ip6_cidr>\d*)){0,1}");

        private readonly IDomainSpecParser _domainSpecParser;
        private readonly IDualCidrBlockParser _dualCidrBlockParser;

        public DomainSpecDualCidrBlockMechanismParser(IDomainSpecParser domainSpecParser,
            IDualCidrBlockParser dualCidrBlockParser)
        {
            _domainSpecParser = domainSpecParser;
            _dualCidrBlockParser = dualCidrBlockParser;
        }

        public Term Parse(string mechanism, Qualifier qualifier, string arguments, Func<string, Qualifier, DomainSpec, DualCidrBlock, Term> termFactory)
        {
            string domainSpecString = string.Empty;
            string ip4CidrString = string.Empty;
            string ip6CidrString = string.Empty;
            if (!string.IsNullOrEmpty(arguments))
            {
                Match match = _regex.Match(arguments);
                if (match.Success)
                {
                    domainSpecString = match.Groups["domain_spec"].Value;
                    ip4CidrString = match.Groups["ip4_cidr"].Value;
                    ip6CidrString = match.Groups["ip6_cidr"].Value;
                }
            }
          
            DomainSpec domainSpec = _domainSpecParser.Parse(domainSpecString, false);
            DualCidrBlock dualCiderBlock = _dualCidrBlockParser.Parse(ip4CidrString, ip6CidrString);

            return termFactory(mechanism, qualifier, domainSpec, dualCiderBlock);
        }
    }
}