using System.Collections.Generic;

namespace Dmarc.DnsRecord.Evaluator.Seeding.Mapping
{
    public interface IMapper<TIn, TOut>
    {
        List<TOut> Map(List<TIn> tin);
    }
}