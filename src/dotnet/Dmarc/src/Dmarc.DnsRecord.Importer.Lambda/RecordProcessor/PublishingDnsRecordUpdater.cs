using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Mapping;
using Dmarc.Common.Interface.Messaging;
using Dmarc.DnsRecord.Contract.Messages;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;

namespace Dmarc.DnsRecord.Importer.Lambda.RecordProcessor
{
    public class PublishingDnsRecordUpdater : IDnsRecordUpdater
    {
        private readonly IDnsRecordUpdater _dnsRecordUpdater;
        private readonly IMapper<List<RecordEntity>, DnsRecordMessage> _mapper;
        private readonly IPublisher _publisher;
        private readonly IPublisherConfig _config;

        public PublishingDnsRecordUpdater(IDnsRecordUpdater dnsRecordUpdater,
            IMapper<List<RecordEntity>, DnsRecordMessage> mapper,
            IPublisher publisher, 
            IPublisherConfig config)
        {
            _dnsRecordUpdater = dnsRecordUpdater;
            _mapper = mapper;
            _publisher = publisher;
            _config = config;
        }

        public async Task<List<RecordEntity>> UpdateRecord(Dictionary<DomainEntity, List<RecordEntity>> records)
        {
            List<RecordEntity> recordEntities = await _dnsRecordUpdater.UpdateRecord(records);

            //Dont publish expired records
            DnsRecordMessage dnsRecordMessage = _mapper.Map(recordEntities.Where(_ => !_.EndDate.HasValue).ToList());

            if (dnsRecordMessage != null)
            {
                await _publisher.Publish(dnsRecordMessage, _config.PublisherConnectionString);
            }

            return recordEntities;
        }
    }
}