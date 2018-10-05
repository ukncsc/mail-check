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
    public class DmarcConfigsUpdatedMapper : IMapper<List<RecordEntity>, DmarcConfigsUpdated>
    {
        public DmarcConfigsUpdated Map(List<RecordEntity> t)
        {
            List<DmarcConfig> dmarcConfigs = t.GroupBy(_ => _.Domain).Select(MapConfig).ToList();

            return dmarcConfigs.Any()
                ? new DmarcConfigsUpdated(dmarcConfigs)
                : null;
        }

        private DmarcConfig MapConfig(IGrouping<DomainEntity, RecordEntity> config)
        {
            Domain domain = new Domain(config.Key.Id, config.Key.Name);
            RecordEntity recordEntity  = config.FirstOrDefault();
            string orgDomain = null;
            bool isTls = false;
            bool isInherited = false;

            if (recordEntity != null)
            {
                orgDomain = ((DmarcRecordInfo)recordEntity.RecordInfo).OrgDomain;
                isTls = ((DmarcRecordInfo)recordEntity.RecordInfo).IsTld;
                isInherited = ((DmarcRecordInfo) recordEntity.RecordInfo).IsInherited;

            }

            List <string> records = config.Select(_ => _.RecordInfo)
                .OfType<DmarcRecordInfo>()
                .Select(_ => _.Record)
                .Where(_ => _ != null)
                .ToList();

            return new DmarcConfig(domain, records, DateTime.UtcNow, orgDomain, isTls, isInherited);
        }
    }
}
