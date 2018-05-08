using Amazon.SQS.Model;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Messaging.Sns.Models;
using Dmarc.DnsRecord.Contract.Domain;
using Dmarc.DnsRecord.Contract.Messages;
using Dmarc.DnsRecord.Evaluator.Spf.Dao;
using Dmarc.DnsRecord.Evaluator.Spf.Dao.Entities;
using Dmarc.DnsRecord.Evaluator.Spf.Mapping;
using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.DnsRecord.Evaluator
{
    public interface ISpfRecordProcessor
    {
        Task Run();
    }

    public class SpfRecordProcessor : ISpfRecordProcessor
    {
        private readonly IQueueProcessor<Message> _queueProcessor;
        private readonly ISpfConfigReadModelDao _dao;
        private readonly ISpfConfigParser _configParser;

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public SpfRecordProcessor(IQueueProcessor<Message> queueProcessor,
            ISpfConfigReadModelDao dao,
            ISpfConfigParser configParser)
        {
            _queueProcessor = queueProcessor;
            _dao = dao;
            _configParser = configParser;
        }

        public async Task Run()
        {
            await _queueProcessor.Run(Process);
        }

        private async Task Process(Message message)
        {
            SnsMessage snsMessage = JsonConvert.DeserializeObject<SnsMessage>(message.Body);
            SpfConfigsUpdated spfConfigsUpdated = JsonConvert.DeserializeObject<SpfConfigsUpdated>(snsMessage.Message);
            List<SpfConfigReadModelEntity> readModelEntities = spfConfigsUpdated.SpfConfigs.Select(Process).ToList();
            await _dao.InsertOrUpdate(readModelEntities);
        }

        private SpfConfigReadModelEntity Process(SpfConfig spfConfig)
        {
            Spf.Domain.SpfConfig parsedConfig = _configParser.Parse(spfConfig);
            Spf.ReadModel.SpfConfig mappedConfig = parsedConfig.ToSpfConfig();
            string spfConfigSerialized = JsonConvert.SerializeObject(mappedConfig, _serializerSettings);
            return new SpfConfigReadModelEntity(spfConfig.Domain.Id, mappedConfig.TotalErrorCount, (ErrorType?)mappedConfig.MaxErrorSeverity, spfConfigSerialized);
        }
    }
}
