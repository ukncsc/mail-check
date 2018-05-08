using Amazon.SQS.Model;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Messaging.Sns.Models;
using Dmarc.DnsRecord.Contract.Domain;
using Dmarc.DnsRecord.Contract.Messages;
using Dmarc.DnsRecord.Evaluator.Dmarc.Dao;
using Dmarc.DnsRecord.Evaluator.Dmarc.Dao.Entities;
using Dmarc.DnsRecord.Evaluator.Dmarc.Mapping;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.DnsRecord.Evaluator
{
    public interface IDmarcRecordProcessor
    {
        Task Run();
    }

    public class DmarcRecordProcessor : IDmarcRecordProcessor
    {
        private readonly IQueueProcessor<Message> _queueProcessor;
        private readonly IDmarcConfigReadModelDao _dmarcConfigReadModelDao;
        private readonly IDmarcConfigParser _dmarcConfigParser;

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public DmarcRecordProcessor(IQueueProcessor<Message> queueProcessor,
            IDmarcConfigReadModelDao dmarcConfigReadModelDao,
            IDmarcConfigParser dmarcConfigParser)
        {
            _queueProcessor = queueProcessor;
            _dmarcConfigReadModelDao = dmarcConfigReadModelDao;
            _dmarcConfigParser = dmarcConfigParser;
        }

        public async Task Run()
        {
            await _queueProcessor.Run(Process);
        }

        private async Task Process(Message message)
        {
            SnsMessage snsMessage = JsonConvert.DeserializeObject<SnsMessage>(message.Body);
            DmarcConfigsUpdated dmarcConfigsUpdated = JsonConvert.DeserializeObject<DmarcConfigsUpdated>(snsMessage.Message);
            List<DmarcConfigReadModelEntity> readModelEntities = dmarcConfigsUpdated.DmarcConfigs.Select(Process).ToList();
            await _dmarcConfigReadModelDao.InsertOrUpdate(readModelEntities);
        }

        private DmarcConfigReadModelEntity Process(DmarcConfig dmarcConfig)
        {
            Dmarc.Domain.DmarcConfig parsedConfig = _dmarcConfigParser.Parse(dmarcConfig);
            Dmarc.ReadModel.DmarcConfig mappedConfig = parsedConfig.ToDmarcConfig();
            string spfConfigSerialized = JsonConvert.SerializeObject(mappedConfig, _serializerSettings);
            return new DmarcConfigReadModelEntity(dmarcConfig.Domain.Id, mappedConfig.TotalErrorCount, (ErrorType?)mappedConfig.MaxErrorSeverity, spfConfigSerialized);
        }
    }
}
