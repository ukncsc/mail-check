using System.Text.RegularExpressions;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface IDomainSpecParser
    {
        DomainSpec Parse(string domainSpec, bool mandatory);
    }

    //Note this doesnt expand macros or perform dns look ups
    //
    public class DomainSpecParserPassive : IDomainSpecParser
    {
        //Regex taken from Eddy Minet's .Net implementation http://www.openspf.org/Implementations
        private readonly Regex _macroRegex = new Regex(@"%{1}\{(?<macro_letter>[slodipvhcrt])(?<digits>[0-9]*)(?<transformer>r)?(?<delimiter>[.+,\/_=-]?)\}", RegexOptions.IgnoreCase);
        
        //Credited to bkr : http://stackoverflow.com/questions/11809631/fully-qualified-domain-name-validation
        private readonly Regex _domainRegex = new Regex(@"(?=^.{4,253}$)(^((?!-)[a-zA-Z0-9-_]{1,63}(?<!-)\.)+[a-zA-Z]{2,63}\.?$)");

        public DomainSpec Parse(string domainSpecString, bool mandatory)
        {
            DomainSpec domainSpec = new DomainSpec(domainSpecString);
            if (string.IsNullOrEmpty(domainSpecString))
            {
                if (mandatory)
                {
                    string errorMessgae = string.Format(SpfParserResource.NoValueErrorMessage, "domain");
                    domainSpec.AddError(new Error(ErrorType.Error, errorMessgae));
                }
            }
            else if (!_domainRegex.IsMatch(domainSpecString) && !_macroRegex.IsMatch(domainSpecString))
            {
                string errorMessage = string.Format(SpfParserResource.InvalidValueErrorMessage, "domain or macro", domainSpecString);
                domainSpec.AddError(new Error(ErrorType.Error, errorMessage));
            }

            return domainSpec;
        }
    }
}