using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public class Ip6 : Mechanism
    {
        public Ip6(string value, Qualifier qualifier, Ip6Addr ipAddress, Ip6CidrBlock cidrBlock) 
            : base(value, qualifier)
        {
            IpAddress = ipAddress;
            CidrBlock = cidrBlock;
        }

        public Ip6Addr IpAddress { get; }

        public Ip6CidrBlock CidrBlock { get; }

        public override IReadOnlyList<Error> AllErrors => IpAddress.AllErrors.Concat(CidrBlock.AllErrors.Concat(Errors)).ToArray();

        public override int AllErrorCount => IpAddress.AllErrorCount + CidrBlock.AllErrorCount + ErrorCount;
    }
}