using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters.ReceivedHeader
{
    public interface IReceivedHeaderSplitter
    {
        List<string> Split(string receivedField);
    }

    public class ReceivedHeaderSplitter : IReceivedHeaderSplitter
    {
        private static readonly Regex Regex = CreateRegEx();
        private readonly ILogger _log;

        public ReceivedHeaderSplitter(ILogger log)
        {
            _log = log;
        }

        private static Regex CreateRegEx()
        {
            string splitPattern = @"(^from\s+|\s+by\s+|\s+via\s+|\s+for\s+|\s+with\s+|\s+id\s+|;\s+)";

            const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;

            return new Regex(splitPattern, options, TimeSpan.FromSeconds(1));
        }

        public List<string> Split(string receivedField)
        {
            try
            {
                return Regex.Split(receivedField.ToLower())
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(_ => _.Trim())
                    .ToList();
            }
            catch (Exception)
            {
                _log.Warn("Failed to split received field.");
                return new List<string>();
            }
        }
    }
}