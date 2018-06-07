using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Interface.Mapping;
using Dmarc.DnsRecord.Contract.Domain;
using Dmarc.DnsRecord.Contract.Messages;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;

namespace Dmarc.DnsRecord.Importer.Lambda.Mapping
{
    public class SpfConfigsUpdatedMapper : IMapper<List<RecordEntity>, SpfConfigsUpdated>
    {
        public SpfConfigsUpdated Map(List<RecordEntity> t)
        {
            List<SpfConfig> spfConfigs = t.GroupBy(_ => _.Domain).Select(MapConfig).ToList();

            return spfConfigs.Any()
                ? new SpfConfigsUpdated(spfConfigs)
                : null;
        }

        private SpfConfig MapConfig(IGrouping<DomainEntity, RecordEntity> config)
        {
            Domain domain = new Domain(config.Key.Id, config.Key.Name);

            List<string> records = config.Select(_ => _.RecordInfo)
                .OfType<SpfRecordInfo>()
                .Select(_ => _.Record)
                .Where(_ => _ != null)
                .ToList();

            return new SpfConfig(domain, records, DateTime.UtcNow);
        }
    }
}
