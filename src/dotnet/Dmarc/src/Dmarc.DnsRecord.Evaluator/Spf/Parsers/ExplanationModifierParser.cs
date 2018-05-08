using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public class ExplanationModifierParser : IModifierParserStrategy
    {
        private readonly IDomainSpecParser _domainSpecParser;

        public ExplanationModifierParser(IDomainSpecParser domainSpecParser)
        {
            _domainSpecParser = domainSpecParser;
        }

        public Term Parse(string modifier, string arguments)
        {
            DomainSpec domainSpec = _domainSpecParser.Parse(arguments, true);

            return new Explanation(modifier, domainSpec);
        }

        public string Modifier => "exp";
    }
}