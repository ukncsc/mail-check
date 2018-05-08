using System.Collections.Generic;
using System.Linq;

namespace Dmarc.DnsRecord.Evaluator.Implicit
{
    public interface IImplicitProvider<T>
    {
        List<T> GetImplicitValues(List<T> tags);
    }

    public class ImplicitProvider<T> : IImplicitProvider<T>
    {
        private readonly List<IImplicitProviderStrategy<T>> _strategies;

        public ImplicitProvider(IEnumerable<IImplicitProviderStrategy<T>> strategies)
        {
            _strategies = strategies.ToList();
        }

        public List<T> GetImplicitValues(List<T> tags)
        {
            List<T> implicitValues = new List<T>();
            foreach (IImplicitProviderStrategy<T> strategy in _strategies)
            {
                T t;
                if (strategy.TryGetImplicitTag(tags, out t) && t != null)
                {
                    implicitValues.Add(t);
                }
            }
            return implicitValues;
        }
    }
}
