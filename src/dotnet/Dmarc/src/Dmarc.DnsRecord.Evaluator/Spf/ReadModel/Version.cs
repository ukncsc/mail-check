using System.Collections.Generic;

namespace Dmarc.DnsRecord.Evaluator.Spf.ReadModel
{
    public class Version
    {
        public Version(string value, string explanation, List<Error> errors)
        {
            Value = value;
            Explanation = explanation;
            Errors = errors;
        }

        public string Value { get; }
        public string Explanation { get; }
        public List<Error> Errors { get; }
    }
}