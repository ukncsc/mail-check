using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using Dmarc.AggregateReport.Parser.Lambda.Config;
using Dmarc.Common.Logging;
using Dmarc.Common.Tasks;

namespace Dmarc.AggregateReport.Parser.Lambda.QueueProcessing
{
    internal interface IQueueProcessor
    {
        Task ProcessQueue(ILambdaContext context);
    }

    internal class QueueProcessor : IQueueProcessor
    {
        private readonly ILambdaAggregateReportParserConfig _config;
        private readonly IAmazonSQS _sqsClient;
        private readonly IMessageProcessor _messageProcessor;
        private readonly ILogger _log;
        private readonly ReceiveMessageRequest _receiveMessageRequest;

        public QueueProcessor(ILambdaAggregateReportParserConfig config,
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
            ReceiveMessageResponse receiveMessageResponse;
            do
            {
                receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(_receiveMessageRequest, CancellationToken.None)
                    .TimeoutAfter(_config.TimeoutSqs).ConfigureAwait(false);
                if (receiveMessageResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    _log.Debug($"Received {receiveMessageResponse.Messages.Count} message(s) from sqs");
                    foreach (Message message in receiveMessageResponse.Messages)
                    {
                        if (await _messageProcessor.TryProcessMessage(context, message))
                        {
                            await _sqsClient.DeleteMessageAsync(_config.SqsQueueUrl, message.ReceiptHandle)
                                .TimeoutAfter(_config.TimeoutSqs).ConfigureAwait(false);
                        }
                        else
                        {
                            _log.Error($"Failed to process message with request id {context.AwsRequestId}");
                        }
                    }
                }
                else
                {
                    _log.Error($"Failed to process message from queue with response {receiveMessageResponse.HttpStatusCode}");
                }
            } while (context.RemainingTime >= _config.RemainingTimeTheshold && receiveMessageResponse.Messages.Any());
        }
    }
}