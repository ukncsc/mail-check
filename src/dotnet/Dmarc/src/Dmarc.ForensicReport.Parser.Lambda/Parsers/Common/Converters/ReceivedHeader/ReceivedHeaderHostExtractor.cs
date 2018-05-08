using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters.ReceivedHeader
{
    public interface IReceivedHeaderHostExtractor
    {
        List<string> ExtractHosts(string input);
    }

    public class ReceivedHeaderHostExtractor : IReceivedHeaderHostExtractor
    {
        private static readonly Regex Regex = CreateRegEx();
        private readonly ILogger _log;

        public ReceivedHeaderHostExtractor(ILogger log)
        {
            _log = log;
        }

        private static Regex CreateRegEx()
        {
            const string host = @"(?:xn--[a-z0-9\-]{1,59}|(?:(?:[a-z\u00a1-\uffff0-9]+-?){0,62}[a-z\u00a1-\uffff0-9]{1,63}))";          //credited to https://gist.github.com/HenkPoley/8899766
            const string domain = @"(?:\.(?:xn--[a-z0-9\-]{1,59}|(?:[a-z\u00a1-\uffff0-9]+-?){0,62}[a-z\u00a1-\uffff0-9]{1,63}))*";     //credited to https://gist.github.com/HenkPoley/8899766
            const string tld = @"(?:\.(?:xn--[a-z0-9\-]{1,59}|(?:[a-z\u00a1-\uffff]{2,63})))\.?";                                       //credited to https://gist.github.com/HenkPoley/8899766

            string hostPattern = $@"(?<=(^|\s+|=))(?:{host}{domain}{tld})";

            const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline;

            return new Regex(hostPattern, options, TimeSpan.FromSeconds(1));
        }

        public List<string> ExtractHosts(string input)
        {
            try
            {
                return (from Match match in Regex.Matches(input) select match.Value).Distinct()
                    .Select(_ => _.ToLower())
                    .ToList();
            }
            catch (Exception)
            {
                _log.Warn("Failed to extract ip addresses.");
                return new List<string>();
            }
        }
    }
}