using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Mapping;
using Dmarc.Common.Interface.Messaging;
using Dmarc.DnsRecord.Contract.Messages;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;

namespace Dmarc.DnsRecord.Importer.Lambda.Publisher
{
    public class RecordEntityPublisher : IRecordEntityPublisher
    {
        private readonly IMapper<List<RecordEntity>, DnsRecordMessage> _mapper;
        private readonly IPublisher _publisher;

        public RecordEntityPublisher(IMapper<List<RecordEntity>, DnsRecordMessage> mapper, 
            IPublisher publisher)
        {
            _mapper = mapper;
            _publisher = publisher;
        }
        
        public async Task Publish(List<RecordEntity> recordEntities)
        {
            DnsRecordMessage dnsRecordMessage = _mapper.Map(recordEntities);
            if (dnsRecordMessage != null)
            {
                await _publisher.Publish(dnsRecordMessage);
            }
        }
    }
}