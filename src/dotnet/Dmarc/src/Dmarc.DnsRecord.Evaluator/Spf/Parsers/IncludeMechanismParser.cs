using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public class IncludeMechanismParser : IMechanismParserStrategy
    {
        private readonly IDomainSpecParser _domainSpecParser;

        public IncludeMechanismParser(IDomainSpecParser domainSpecParser)
        {
            _domainSpecParser = domainSpecParser;
        }

        //"include" ":" domain-spec
        public Term Parse(string mechanism, Qualifier qualifier, string arguments)
        {
            DomainSpec domainSpec = _domainSpecParser.Parse(arguments, true);

            return new Include(mechanism, qualifier, domainSpec);
        }

        public string Mechanism => "include";
    }
}