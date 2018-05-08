using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface IMechanismParserStrategy
    {
        string Mechanism { get; }
        Term Parse(string mechanism, Qualifier qualifier, string arguments);
    }
}