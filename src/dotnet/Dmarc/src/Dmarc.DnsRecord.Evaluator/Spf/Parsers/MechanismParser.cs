using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface IMechanismParser
    {
        bool TryParse(string mechanism, out Term term);
    }

    public class MechanismParser : IMechanismParser
    {
        private readonly Regex _mechanismRegex =
            new Regex(
                @"^(?<qualifier>[+?~-]?)(?<mechanism>(all)|(include)|(A)|(MX)|(PTR)|(IP4)|(IP6)|(exists))(:?(?<argument>.+))?$",
                RegexOptions.IgnoreCase);

        private readonly IQualifierParser _qualifierParser;
        private readonly Dictionary<string, IMechanismParserStrategy> _parserStategies;

        public MechanismParser(IQualifierParser qualifierParser,
            IEnumerable<IMechanismParserStrategy> parserStategies)
        {
            _qualifierParser = qualifierParser;
            _parserStategies = parserStategies.ToDictionary(_ => _.Mechanism);
        }

        public bool TryParse(string mechanism, out Term term)
        {
            Match match = _mechanismRegex.Match(mechanism);
            if (match.Success)
            {
                string qualifierToken = match.Groups["qualifier"].Value;
                string mechanismToken = match.Groups["mechanism"].Value.ToLower();
                string argumentToken = match.Groups["argument"].Value;

                Qualifier qualifier = _qualifierParser.Parse(qualifierToken);
                IMechanismParserStrategy strategy;
                if (!_parserStategies.TryGetValue(mechanismToken, out strategy))
                {
                    throw new ArgumentException($"No strategy found to process {mechanismToken}");
                }
                term = strategy.Parse(mechanism, qualifier, argumentToken);
                return true;
            }
            term = null;
            return false;
        }
    }
}