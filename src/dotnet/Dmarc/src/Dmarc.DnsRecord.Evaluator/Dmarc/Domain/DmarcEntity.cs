using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public abstract class DmarcEntity
    {
        private readonly List<Error> _errors = new List<Error>();

        public bool Valid => !_errors.Any();

        public int ErrorCount => _errors.Count;

        public IReadOnlyList<Error> Errors => _errors.ToArray();

        public bool AllValid => AllErrorCount == 0;

        public virtual int AllErrorCount => ErrorCount;

        public virtual IReadOnlyList<Error> AllErrors => Errors;

        public void AddError(Error error)
        {
            _errors.Add(error);
        }

        public void AddErrors(List<Error> errors)
        {
            _errors.AddRange(errors);
        }
    }
}