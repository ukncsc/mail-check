using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Dmarc.Common.Report.Config;
using Amazon.SQS;
using Amazon.SQS.Model;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Tasks;

namespace Dmarc.Common.Report.QueueProcessing
{
    public interface IQueueProcessor
    {
        Task ProcessQueue(ILambdaContext context);
    }

    public class QueueProcessor : IQueueProcessor
    {
        private readonly ILambdaReportParserConfig _config;
        private readonly IAmazonSQS _sqsClient;
        private readonly IMessageProcessor _messageProcessor;
        private readonly ILogger _log;
        private readonly ReceiveMessageRequest _receiveMessageRequest;

        public QueueProcessor(ILambdaReportParserConfig config,
            IAmazonSQS sqsClient, 
            IMessageProcessor messageProcessor,
            ILogger log)
        {
            _config = config;
            _sqsClient = sqsClient;
            _messageProcessor = messageProcessor;
            _log = log;

            _receiveMessageRequest = new ReceiveMessageRequest
            {
                WaitTimeSeconds = 0,
                MaxNumberOfMessages = 1,
                QueueUrl = _config.SqsQueueUrl
            };
        }

        public async Task ProcessQueue(ILambdaContext context)
        {
            _log.Info($"Processing request with Id: {context.AwsRequestId}");
            ReceiveMessageResponse receiveMessageResponse;
            do
            {
                _log.Info($"{context.RemainingTime} time remaining for requestId {context.AwsRequestId}");
                receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(_receiveMessageRequest, CancellationToken.None)
                    .TimeoutAfter(_config.TimeoutSqs).ConfigureAwait(false);

                if (receiveMessageResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    _log.Info($"Received {receiveMessageResponse.Messages.Count} message(s) from sqs");
                    foreach (Message message in receiveMessageResponse.Messages)
                    {
                        _log.Info($"Processing message with message Id: {message.MessageId} for request Id: {context.AwsRequestId}");
                        if (await _messageProcessor.TryProcessMessage(context, message))
                        {
                            _log.Info($"Successfully processed message with message Id: {message.MessageId} for request Id: {context.AwsRequestId}");

                            await _sqsClient.DeleteMessageAsync(_config.SqsQueueUrl, message.ReceiptHandle)
                                .TimeoutAfter(_config.TimeoutSqs).ConfigureAwait(false);

                            _log.Info($"Deleted message with message Id: {message.MessageId} for request Id: {context.AwsRequestId}");
                        }
                        else
                        {
                            _log.Error($"Failed to process message with message Id: {message.MessageId} for request Id: {context.AwsRequestId}");
                        }
                    }
                }
                else
                {
                    _log.Error($"Failed to retrieve message from sqs with response {receiveMessageResponse.HttpStatusCode} for request Id: {context.AwsRequestId}");
                }
            } while (context.RemainingTime >= _config.RemainingTimeTheshold && receiveMessageResponse.Messages.Any());
        }
    }
}