using System.Text.RegularExpressions;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public class VersionParserStrategy : ITagParserStrategy
    {
        private readonly Regex _regex = new Regex("DMARC1$", RegexOptions.IgnoreCase);

        public Tag Parse(string tag, string value)
        {
            Version version = new Version(tag);
            if (!_regex.IsMatch(value ?? string.Empty))
            {
                string errorMessage = string.Format(DmarcParserResource.InvalidValueErrorMessage, Tag, value);
                version.AddError(new Error(ErrorType.Error, errorMessage));
            }
            return version;
        }

        public string Tag => "v";

        public int MaxOccurences => 1;
    }
}