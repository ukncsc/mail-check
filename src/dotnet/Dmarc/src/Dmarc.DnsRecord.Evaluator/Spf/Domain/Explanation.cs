using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public class Explanation : Modifier
    {
        public Explanation(string value, DomainSpec domainSpec) 
            : base(value)
        {
            DomainSpec = domainSpec;
        }

        public DomainSpec DomainSpec { get; }

        public override IReadOnlyList<Error> AllErrors => DomainSpec.AllErrors.Concat(Errors).ToArray();

        public override int AllErrorCount => DomainSpec.AllErrorCount + ErrorCount;
    }
}