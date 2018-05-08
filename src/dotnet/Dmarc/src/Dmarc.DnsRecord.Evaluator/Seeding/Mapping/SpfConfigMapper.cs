using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Linq;
using Dmarc.DnsRecord.Contract.Domain;
using Dmarc.DnsRecord.Contract.Messages;

namespace Dmarc.DnsRecord.Evaluator.Seeding.Mapping
{
    public class SpfConfigMapper : IMapper<SpfRecord, SpfConfigsUpdated>
    {
        private const int BatchSize = 50;

        public List<SpfConfigsUpdated> Map(List<SpfRecord> tin)
        {
            return tin
                .GroupBy(_ => _.Domain.DomainId)
                .Batch(50)
                .Select(batch => 
                    new SpfConfigsUpdated(batch.Select(config => 
                            new SpfConfig(
                                new Domain(config.First().Domain.DomainId, config.First().Domain.DomainName), 
                                config.Select(_ => _.Record)
                                    .ToList()))
                        .ToList()))
                .ToList();
        }
    }
}