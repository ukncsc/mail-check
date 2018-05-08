using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface IModifierParserStrategy
    {
        Term Parse(string modifier, string arguments);
        string Modifier { get; }
    }
}