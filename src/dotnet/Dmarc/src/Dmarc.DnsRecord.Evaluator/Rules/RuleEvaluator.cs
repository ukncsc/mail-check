using System.Collections.Generic;
using System.Linq;

namespace Dmarc.DnsRecord.Evaluator.Rules
{
    public interface IRuleEvaluator<in T>
    {
        List<Error> Evaluate(T t);
    }

    public class RuleEvaluator<T> : IRuleEvaluator<T>
    {
        private readonly List<IRule<T>> _rules;

        public RuleEvaluator(IEnumerable<IRule<T>> rules)
        {
            _rules = rules.ToList();
        }

        public List<Error> Evaluate(T t)
        {
            List<Error> errors = new List<Error>();
            foreach (IRule<T> rule in _rules)
            {
                Error error;
                if (rule.IsErrored(t, out error))
                {
                    errors.Add(error);
                }
            }
            return errors;
        }
    }
}