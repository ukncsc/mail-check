using System.Text.RegularExpressions;

namespace Dmarc.Common.Validation
{
    public interface IDomainValidator
    {
        bool IsValidDomain(string domain);
    }

    public class DomainValidator : IDomainValidator
    {
        //Credited to bkr : http://stackoverflow.com/questions/11809631/fully-qualified-domain-name-validation
        private readonly Regex _regex = new Regex(@"(?=^.{4,253}$)(^((?!-)[a-zA-Z0-9-]{1,63}(?<!-)\.)+[a-zA-Z]{2,63}\.?$)");

        public bool IsValidDomain(string domain) => _regex.Match(domain?.ToLower() ?? string.Empty).Success;
    }
}
