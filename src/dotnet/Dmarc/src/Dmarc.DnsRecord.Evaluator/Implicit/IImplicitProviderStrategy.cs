using System.Collections.Generic;

namespace Dmarc.DnsRecord.Evaluator.Implicit
{
    public interface IImplicitProviderStrategy<T>
    {
        bool TryGetImplicitTag(List<T> ts, out T t);
    }
}