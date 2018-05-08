using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Linq;
using Dmarc.DnsRecord.Contract.Domain;
using Dmarc.DnsRecord.Contract.Messages;
using Dmarc.DnsRecord.Evaluator.Seeding.Mapping;

namespace Dmarc.DnsRecord.Evaluator.Seeding
{
    public class DmarcConfigMapper : IMapper<DmarcRecord, DmarcConfigsUpdated>
    {
        private const int BatchSize = 50;

        public List<DmarcConfigsUpdated> Map(List<DmarcRecord> tin)
        {
            return tin
                .GroupBy(_ => _.Domain.DomainId)
                .Batch(50)
                .Select(batch =>
                    new DmarcConfigsUpdated(batch.Select(config =>
                            new DmarcConfig(
                                new Contract.Domain.Domain(config.First().Domain.DomainId, config.First().Domain.DomainName),
                                config.Select(_ => _.Record)
                                    .ToList()))
                        .ToList()))
                .ToList();
        }
    }
}