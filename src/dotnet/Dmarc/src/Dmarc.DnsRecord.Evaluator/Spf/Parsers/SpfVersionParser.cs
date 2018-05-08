using System.Text.RegularExpressions;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface ISpfVersionParser
    {
        Version Parse(string version);
    }

    public class SpfVersionParser : ISpfVersionParser
    {
        private readonly Regex _regex = new Regex("^v=spf1$", RegexOptions.IgnoreCase);

        public Version Parse(string versionString)
        {
            Version version = new Version(versionString);
            if (versionString == null || !_regex.IsMatch(versionString))
            {
                string errorMessage = string.Format(SpfParserResource.InvalidValueErrorMessage, "SPF version", version);
                version.AddError(new Error(ErrorType.Error, errorMessage));
            }
            return version;
        }
    }
}