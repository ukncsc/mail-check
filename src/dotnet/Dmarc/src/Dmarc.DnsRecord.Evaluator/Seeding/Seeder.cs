using Amazon.SQS;
using Dmarc.Common.Messaging.Sns.Models;
using Dmarc.DnsRecord.Evaluator.Seeding.Config;
using Dmarc.DnsRecord.Evaluator.Seeding.Dao;
using Dmarc.DnsRecord.Evaluator.Seeding.Mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dmarc.DnsRecord.Evaluator.Seeding
{
    public interface ISeeder
    {
        Task SeedData();
    }

    public class Seeder<TIn, TOut> : ISeeder
    {
        private readonly IDnsRecordDao<TIn> _dao;
        private readonly IMapper<TIn, TOut> _mapper;
        private readonly IAmazonSQS _sqsClient;
        private readonly ISeedingConfig _config;

        public Seeder(IDnsRecordDao<TIn> dao, 
            IMapper<TIn, TOut> mapper,
            IAmazonSQS sqsClient,
            ISeedingConfig config)
        {
            _dao = dao;
            _mapper = mapper;
            _sqsClient = sqsClient;
            _config = config;
        }

        public async Task SeedData()
        {
            List<TIn> ins = await _dao.GetCurrentRecords();
            List<TOut> outs = _mapper.Map(ins);

            List<string> serializedOuts = outs.Select(_ => JsonConvert.SerializeObject(_)).ToList();

            List<SnsMessage> snsMessages = serializedOuts.Select(_ => new SnsMessage(
                string.Empty, string.Empty, string.Empty, _, DateTime.UtcNow, 0, string.Empty, string.Empty)).ToList();

            List<string> serializedSnsMessages = snsMessages.Select(JsonConvert.SerializeObject).ToList();

            foreach (var message in serializedSnsMessages)
            {
                await _sqsClient.SendMessageAsync(_config.SqsQueueUrl, message, CancellationToken.None);
            }
        }
    }
}
