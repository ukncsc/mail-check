using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Rules
{
    public interface IRule<in T>
    {
        bool IsErrored(T record, out Error error);
    }
}