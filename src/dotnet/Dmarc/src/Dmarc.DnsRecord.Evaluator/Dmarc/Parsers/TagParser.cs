using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public interface ITagParser
    {
        List<Tag> Parse(List<string> stringTags);
    }

    public class TagParser : ITagParser
    {
        private const char Separator = '=';
        private readonly Dictionary<string, ITagParserStrategy> _strategies;

        public TagParser(IEnumerable<ITagParserStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(_ => _.Tag);
        }

        public List<Tag> Parse(List<string> stringTags)
        {
            List<Tag> tags = new List<Tag>();
            foreach (string stringTag in stringTags)
            {
                Parse(stringTag, tags);
            }
            return tags;
        }

        private void Parse(string stringTag, List<Tag> tags)
        {
            string[] tokens = stringTag?.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries) ??
                              new string[0];

            ITagParserStrategy strategy;
            if (_strategies.TryGetValue(tokens.ElementAtOrDefault(0)?.ToLower() ?? string.Empty, out strategy))
            {
                Tag tag = strategy.Parse(stringTag, tokens.ElementAtOrDefault(1));

                int tagOccurences = tags.Where(_ => _.GetType() == tag.GetType()).Count();

                if (tagOccurences >= strategy.MaxOccurences)
                {
                    tag.AddError(new Error(ErrorType.Error,
                        $"The {strategy.Tag} tag should occur no more than {GetOccurrences(strategy.MaxOccurences)}." +
                        $" This record has {tagOccurences + 1} occurrences."));
                }

                if (tokens.Length > 2)
                {
                    string unexpectedValues = string.Join(",", tokens.Skip(2));
                    string unexpectedValuesErrorMessage =
                        string.Format(DmarcParserResource.UnexpectedValueErrorMessage, unexpectedValues, "term",
                            stringTag);
                    tag.AddError(new Error(ErrorType.Error, unexpectedValuesErrorMessage));
                }

                tags.Add(tag);
            }
            else
            {

                UnknownTag unknownTag = new UnknownTag(tokens.ElementAtOrDefault(0), tokens.ElementAtOrDefault(1));

                string unknownTagErrorMessage = string.Format(DmarcParserResource.UnknownTagErrorMessage,
                    tokens.ElementAtOrDefault(0) ?? "<null>", tokens.ElementAtOrDefault(1) ?? "<null>");

                unknownTag.AddError(new Error(ErrorType.Error, unknownTagErrorMessage));

                tags.Add(unknownTag);
            }
        }

        public string GetOccurrences(int occurrences)
        {
            return occurrences == 1 ? "once" : (occurrences == 2 ? "twice" : $"{occurrences} times");
        }
    }
}