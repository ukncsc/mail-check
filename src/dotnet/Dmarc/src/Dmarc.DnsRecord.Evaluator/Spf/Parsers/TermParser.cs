using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface ITermParser
    {
        Term Parse(string term);
    }

    public class TermParser : ITermParser
    {
        private readonly IMechanismParser _mechanismParser;
        private readonly IModifierParser _modifierParser;

        public TermParser(IMechanismParser mechanismParser, 
            IModifierParser modifierParser)
        {
            _mechanismParser = mechanismParser;
            _modifierParser = modifierParser;
        }

        public Term Parse(string stringTerm)
        {
            Term term;
            if (_mechanismParser.TryParse(stringTerm, out term))
            {
                return term;
            }

            if(_modifierParser.TryParse(stringTerm, out term))
            {
                return term;
            }

            UnknownTerm unknownTerm = new UnknownTerm(stringTerm);
            string errorMessage = string.Format(SpfParserResource.UnknownTermErrorMessage, stringTerm);
            unknownTerm.AddError(new Error(ErrorType.Error, errorMessage));
            return unknownTerm;
        }
    }
}