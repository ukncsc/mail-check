using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Urls
{
    public interface IUrlExtractor
    {
        List<string> ExtractUrls(string input);
    }

    public class UrlExtractor : IUrlExtractor
    {
        private readonly ILogger _log;
        private static readonly Regex Regex = CreateRegEx();

        public UrlExtractor(ILogger log)
        {
            _log = log;
        }

        private static Regex CreateRegEx()
        {
            string validChar = @"a-z0-9-._~:/?#\[\]@!$&'()*+,;=%";
            string regex = $"(?<=(?:^|[^{validChar}]))(?:(?:https?|ftp)://)[{validChar}]*?(?=(?:([^{validChar}]|$)))";

            const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Multiline;

            return new Regex(regex, options, TimeSpan.FromSeconds(1));
        }
        
        public List<string> ExtractUrls(string input)
        {
            try
            {
                return (from Match match in Regex.Matches(input) select match.Value).Distinct().ToList();
            }
            catch (Exception)
            {
                _log.Warn("Failed to extract urls.");
                return new List<string>();
            }
        }
    }
}