using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public interface ITagParserStrategy
    {
        Tag Parse(string tag, string value);
        string Tag { get; }
        int MaxOccurences { get; }
    }
}