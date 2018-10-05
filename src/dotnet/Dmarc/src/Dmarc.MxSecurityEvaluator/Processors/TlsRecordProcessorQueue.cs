using System.Threading.Tasks;
using Amazon.SQS.Model;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Messaging.Sns.Models;
using Dmarc.MxSecurityEvaluator.Dao;
using Dmarc.MxSecurityTester.Contract.Messages;
using static Newtonsoft.Json.JsonConvert;

namespace Dmarc.MxSecurityEvaluator.Processors
{
    public class TlsRecordProcessorQueue : TlsRecordProcessor
    {
        private readonly IQueueProcessor<Message> _queueProcessor;

        public TlsRecordProcessorQueue(ITlsRecordDao tlsRecordDao, IMxSecurityEvaluator mxSecurityEvaluator,
            ILogger log, IQueueProcessor<Message> queueProcessor) :
            base(tlsRecordDao, mxSecurityEvaluator, log)
        {
            _queueProcessor = queueProcessor;
        }

        public override async Task Run()
        {
            await _queueProcessor.Run(Process);
        }

        private async Task Process(Message message)
        {
            DomainTlsProfileChanged domain = GetSnsMessageBody(message);

            await ProcessTlsConnectionResults(domain.DomainId);
        }

        private static DomainTlsProfileChanged GetSnsMessageBody(Message message)
        {
            SnsMessage snsMessage = DeserializeObject<SnsMessage>(message.Body);

            return DeserializeObject<DomainTlsProfileChanged>(snsMessage.Message);
        }
    }
}
