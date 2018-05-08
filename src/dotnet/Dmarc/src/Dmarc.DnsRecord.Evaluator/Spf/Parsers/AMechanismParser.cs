using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public class AMechanismParser : IMechanismParserStrategy
    {
        private readonly IDomainSpecDualCidrBlockMechanismParser _mechanismParser;

        public AMechanismParser(IDomainSpecDualCidrBlockMechanismParser mechanismParser)
        {
            _mechanismParser = mechanismParser;
        }

        // "a" [ ":" domain-spec ] [ dual-cidr-length ]
        public Term Parse(string mechanism, Qualifier qualifier, string arguments)
        {
            return _mechanismParser.Parse(mechanism, qualifier, arguments, (s, q, ds, dc) => new A(s, q, ds, dc));
        }

        public string Mechanism => "a";
    }
}