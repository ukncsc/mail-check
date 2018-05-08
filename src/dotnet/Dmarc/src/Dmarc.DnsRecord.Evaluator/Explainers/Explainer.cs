using System;
using System.Collections.Generic;
using System.Linq;

namespace Dmarc.DnsRecord.Evaluator.Explainers
{
    public interface IExplainer<in T>
    {
        bool TryExplain(T t, out string explanation);
    }

    public class Explainer<T> : IExplainer<T>
    {
        private readonly Dictionary<Type,IExplainerStrategy<T>> _tagExplainerStrategies;

        public Explainer(IEnumerable<IExplainerStrategy<T>> tagExplainerStrategies)
        {
            _tagExplainerStrategies = tagExplainerStrategies.ToDictionary(_ => _.Type);
        }

        public bool TryExplain(T t, out string explanation)
        {
            IExplainerStrategy<T> strategy;
            if (_tagExplainerStrategies.TryGetValue(t.GetType(), out strategy))
            {
                return strategy.TryExplain(t, out explanation);
            }
            explanation = null;
            return false;
        }
    }
}
