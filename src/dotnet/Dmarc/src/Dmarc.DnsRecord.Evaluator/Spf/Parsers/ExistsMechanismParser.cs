using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public class ExistsMechanismParser : IMechanismParserStrategy
    {
        private readonly IDomainSpecParser _domainSpecParser;

        public ExistsMechanismParser(IDomainSpecParser domainSpecParser)
        {
            _domainSpecParser = domainSpecParser;
        }

        //"include" ":" domain-spec
        public Term Parse(string mechanism, Qualifier qualifier, string arguments)
        {
            DomainSpec domainSpec = _domainSpecParser.Parse(arguments, true);

            return new Exists(mechanism, qualifier, domainSpec);
        }

        public string Mechanism => "exists";
    }
}