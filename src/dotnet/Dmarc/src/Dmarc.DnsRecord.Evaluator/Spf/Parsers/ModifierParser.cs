using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface IModifierParser
    {
        bool TryParse(string modifier, out Term term);
    }

    public class ModifierParser : IModifierParser
    {
        private readonly Regex _modifierRegex = new Regex(@"^(?<modifier>[^=:/]+)=(?<argument>.+)$", RegexOptions.IgnoreCase);

        private readonly Dictionary<string, IModifierParserStrategy> _modifierParserStrategies;


        public ModifierParser(IEnumerable<IModifierParserStrategy> modifierParserStrategies)
        {
            _modifierParserStrategies = modifierParserStrategies.ToDictionary(_ => _.Modifier);
        }

        public bool TryParse(string modifier, out Term term)
        {
            Match match = _modifierRegex.Match(modifier);
            if (match.Success)
            {
                string modifierToken = match.Groups["modifier"].Value.ToLower();
                string argumentToken = match.Groups["argument"].Value;

                IModifierParserStrategy strategy;

                term = _modifierParserStrategies.TryGetValue(modifierToken, out strategy)
                    ? strategy.Parse(modifier, argumentToken)
                    : new UnknownModifier(modifier);

                return true;
            }
            term = null;
            return false;
        }
    }
}