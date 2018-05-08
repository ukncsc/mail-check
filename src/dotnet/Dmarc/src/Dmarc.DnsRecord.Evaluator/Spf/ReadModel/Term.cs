using System.Collections.Generic;

namespace Dmarc.DnsRecord.Evaluator.Spf.ReadModel
{
    public class Term
    {
        public Term(string value, string explanation, bool isImplicit, List<Error> errors)
        {
            Value = value;
            Explanation = explanation;
            IsImplicit = isImplicit;
            Errors = errors;
        }

        public string Value { get; }
        public string Explanation { get; }
        public bool IsImplicit { get; }
        public List<Error> Errors { get; }
    }
}
