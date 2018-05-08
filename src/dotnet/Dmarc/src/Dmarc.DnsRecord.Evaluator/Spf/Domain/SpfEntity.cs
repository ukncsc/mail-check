using System.Collections.Generic;
using System.Linq;

namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public abstract class SpfEntity
    {
        private readonly List<Evaluator.Rules.Error> _errors = new List<Evaluator.Rules.Error>();

        public bool Valid => !_errors.Any();

        public int ErrorCount => _errors.Count;

        public IReadOnlyList<Evaluator.Rules.Error> Errors => _errors.ToArray();

        public bool AllValid => AllErrorCount == 0;

        public virtual int AllErrorCount => ErrorCount;

        public virtual IReadOnlyList<Evaluator.Rules.Error> AllErrors => Errors;

        public void AddError(Evaluator.Rules.Error error)
        {
            _errors.Add(error);
        }

        public void AddErrors(List<Evaluator.Rules.Error> errors)
        {
            _errors.AddRange(errors);
        }
    }
}