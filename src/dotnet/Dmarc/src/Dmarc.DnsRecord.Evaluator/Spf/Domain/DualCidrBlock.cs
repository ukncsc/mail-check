using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public class DualCidrBlock : SpfEntity
    {
        public DualCidrBlock(Ip4CidrBlock ip4CidrBlock, Ip6CidrBlock ip6CidrBlock)
        {
            Ip4CidrBlock = ip4CidrBlock;
            Ip6CidrBlock = ip6CidrBlock;
        }

        public Ip4CidrBlock Ip4CidrBlock { get; }
        public Ip6CidrBlock Ip6CidrBlock { get; }

        public override IReadOnlyList<Error> AllErrors => Ip4CidrBlock.AllErrors.Concat(Ip6CidrBlock.AllErrors).ToArray();

        public override int AllErrorCount => Ip4CidrBlock.AllErrorCount + Ip6CidrBlock.AllErrorCount;
    }
}