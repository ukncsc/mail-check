using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.Common.Tasks;
using Dmarc.Common.Interface.Messaging;

namespace Dmarc.Common.Messaging.Sqs.QueueProcessor
{
    public class SqsLongPollingQueueProcessor : IQueueProcessor<Message>
    {
        private readonly TimeSpan _receiveMessageTimeout = TimeSpan.FromSeconds(25);
        private readonly TimeSpan _deleteMesssageTimeout = TimeSpan.FromSeconds(5);

        private readonly ISqsConfig _config;
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger _log;
        private readonly ReceiveMessageRequest _receiveMessageRequest;

        public SqsLongPollingQueueProcessor(ISqsConfig config, IAmazonSQS sqsClient, ILogger log)
        {
            _config = config;
            _sqsClient = sqsClient;
            _log = log;
            _receiveMessageRequest = new ReceiveMessageRequest
            {
                WaitTimeSeconds = config.WaitTimeSeconds,
                MaxNumberOfMessages = config.MaxNumberOfMessages,
                QueueUrl = config.QueueUrl
            };
        }

        public async Task Run(Func<Message, Task> job)
        {
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                stopwatch.Start();
                _log.Debug($"Starting long poll of sqs queue {_config.QueueUrl}");

                ReceiveMessageResponse response = await _sqsClient
                    .ReceiveMessageAsync(_receiveMessageRequest, CancellationToken.None)
                    .TimeoutAfter(_receiveMessageTimeout).ConfigureAwait(false);

                _log.Debug($"Completed long poll with result {response.HttpStatusCode} in {stopwatch.Elapsed}");

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    _log.Debug($"Found {response.Messages.Count} messages.");

                    foreach (Message message in response.Messages)
                    {
                        try
                        {
                            await job(message);
                            await _sqsClient
                                .DeleteMessageAsync(_config.QueueUrl, message.ReceiptHandle)
                                .TimeoutAfter(_deleteMesssageTimeout).ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            _log.Error($"Failed process message with error: {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                        }
                    }
                }
                else
                {
                    _log.Error($"Failed to read message from queue with response {response.HttpStatusCode}");
                }
                _log.Debug($"Completed poll and processing in {stopwatch.Elapsed}");
                stopwatch.Restart();
            }
        }
    }
}
