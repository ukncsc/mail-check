using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using Dmarc.AggregateReport.Parser.Lambda.Config;
using Dmarc.AggregateReport.Parser.Lambda.QueueProcessing;
using Dmarc.Common.Logging;
using Dmarc.Lambda.AggregateReport.Parser.Test.Util;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.QueueProcessing
{
    [TestFixture]
    public class AggregateReportQueueProcessorTests
    {
        private QueueProcessor _queueProcessor;
        private IAmazonSQS _sqsClient;
        private IMessageProcessor _messageProcessor;
        private ILogger _log;
        private ILambdaAggregateReportParserConfig _lambdaAggregateReportParserConfig;

        [SetUp]
        public void SetUp()
        {
            _lambdaAggregateReportParserConfig = A.Fake<ILambdaAggregateReportParserConfig>();
            A.CallTo(() => _lambdaAggregateReportParserConfig.RemainingTimeTheshold).Returns(TimeSpan.FromSeconds(10));
            _sqsClient = A.Fake<IAmazonSQS>();
            _messageProcessor = A.Fake<IMessageProcessor>();
            _log = A.Fake<ILogger>();
            _queueProcessor = new QueueProcessor(_lambdaAggregateReportParserConfig, _sqsClient, _messageProcessor, _log);
        }

        [Test]
        public async Task ProcessQueueAtLeastOneAttemptMadeToReceiveMessage()
        {
            await _queueProcessor.ProcessQueue(A.Fake<ILambdaContext>());
            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task ProcessQueueResponseDoesntHaveHttpStatusOkDontProcessDontDeleteLogError()
        {
            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
                .Returns(CreateReceiveMessageResponse(HttpStatusCode.BadRequest));

            await _queueProcessor.ProcessQueue(A.Fake<ILambdaContext>());

            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => _messageProcessor.TryProcessMessage(A<ILambdaContext>._, A<Message>._)).MustNotHaveHappened();
            A.CallTo(() => _sqsClient.DeleteMessageAsync(A<string>._, A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _log.Error(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task ProcessQueueResponseMessageProcessFailureDontDeleteMessageLogError()
        {
            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
                  .Returns(CreateReceiveMessageResponse(HttpStatusCode.OK, new List<Message> {new Message()}));

            A.CallTo(() => _messageProcessor.TryProcessMessage(A<ILambdaContext>._, A<Message>._))
                .Returns(Task.FromResult(false));

            await _queueProcessor.ProcessQueue(A.Fake<ILambdaContext>());

            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._)).MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => _sqsClient.DeleteMessageAsync(A<string>._, A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _log.Error(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task ProcessQueueResponseMessageProcessSuccessDeleteMessage()
        {
            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
                  .Returns(CreateReceiveMessageResponse(HttpStatusCode.OK, new List<Message> { new Message() }));

            A.CallTo(() => _messageProcessor.TryProcessMessage(A<ILambdaContext>._, A<Message>._))
                .Returns(Task.FromResult(true));

            await _queueProcessor.ProcessQueue(A.Fake<ILambdaContext>());

            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._)).MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => _sqsClient.DeleteMessageAsync(A<string>._, A<string>._, A<CancellationToken>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task ProcessQueueNotSufficientTimeRemainingStopProcessingQueue()
        {
            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
                  .Returns(CreateReceiveMessageResponse(HttpStatusCode.OK, new List<Message> { new Message() }));

            A.CallTo(() => _messageProcessor.TryProcessMessage(A<ILambdaContext>._, A<Message>._))
                .Returns(Task.FromResult(true));

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            A.CallTo(() => lambdaContext.RemainingTime).Returns(TimeSpan.FromSeconds(4));

            await _queueProcessor.ProcessQueue(lambdaContext);

            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._)).MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => _sqsClient.DeleteMessageAsync(A<string>._, A<string>._, A<CancellationToken>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task ProcessQueueSufficientTimeRemainingKeepProcessingQueue()
        {
            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
                 .Returns(CreateReceiveMessageResponse(HttpStatusCode.OK, new List<Message> { new Message() }));

            A.CallTo(() => _messageProcessor.TryProcessMessage(A<ILambdaContext>._, A<Message>._))
                .Returns(Task.FromResult(true));

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            A.CallTo(() => lambdaContext.RemainingTime).ReturnsNextFromSequence(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(9));

            await _queueProcessor.ProcessQueue(lambdaContext);

            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._)).MustHaveHappened(Repeated.Exactly.Twice);

            A.CallTo(() => _sqsClient.DeleteMessageAsync(A<string>._, A<string>._, A<CancellationToken>._)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public async Task ProcessQueueNoMoreMessagesStopProcessingQueue()
        {
            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
                 .Returns(CreateReceiveMessageResponse(HttpStatusCode.OK));

            A.CallTo(() => _messageProcessor.TryProcessMessage(A<ILambdaContext>._, A<Message>._))
                .Returns(Task.FromResult(true));

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            A.CallTo(() => lambdaContext.RemainingTime).ReturnsNextFromSequence(TimeSpan.FromSeconds(6));

            await _queueProcessor.ProcessQueue(lambdaContext);

            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._)).MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => _sqsClient.DeleteMessageAsync(A<string>._, A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task ProcessQueueAreMoreMessagesKeepProcessingQueue()
        {
            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
                 .ReturnsNextFromSequence(CreateReceiveMessageResponse(HttpStatusCode.OK, new List<Message> { new Message() }),
                 CreateReceiveMessageResponse(HttpStatusCode.OK));

            A.CallTo(() => _messageProcessor.TryProcessMessage(A<ILambdaContext>._, A<Message>._))
                .Returns(Task.FromResult(true));

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            A.CallTo(() => lambdaContext.RemainingTime).ReturnsNextFromSequence(TimeSpan.FromSeconds(20));

            await _queueProcessor.ProcessQueue(lambdaContext);

            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._)).MustHaveHappened(Repeated.Exactly.Twice);

            A.CallTo(() => _sqsClient.DeleteMessageAsync(A<string>._, A<string>._, A<CancellationToken>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task ProcessQueueThrowsIfQueueReadTimesout()
        {
            A.CallTo(() => _lambdaAggregateReportParserConfig.TimeoutSqs).Returns(TimeSpan.FromTicks(1));
            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
                .Returns(new TaskCompletionSource<ReceiveMessageResponse>().Task);

            await AssertEx.ThrowsAsync<TimeoutException>(() => _queueProcessor.ProcessQueue(A.Fake<ILambdaContext>()));
        }

        [Test]
        public async Task ProcessQueueThrowsIfQueueDeleteTimesout()
        {
            A.CallTo(() => _lambdaAggregateReportParserConfig.TimeoutSqs).Returns(TimeSpan.FromTicks(1));

            A.CallTo(() => _sqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
                  .Returns(CreateReceiveMessageResponse(HttpStatusCode.OK, new List<Message> { new Message() }));

            A.CallTo(() => _messageProcessor.TryProcessMessage(A<ILambdaContext>._, A<Message>._))
                .Returns(Task.FromResult(true));

            A.CallTo(() => _sqsClient.DeleteMessageAsync(A<string>._, A<string>._, A<CancellationToken>._))
                .Returns(new TaskCompletionSource<DeleteMessageResponse>().Task);

            await AssertEx.ThrowsAsync<TimeoutException>(() => _queueProcessor.ProcessQueue(A.Fake<ILambdaContext>()));
        }

        private static Task<ReceiveMessageResponse> CreateReceiveMessageResponse(HttpStatusCode httpStatusCode, List<Message> messages = null)
        {
            return Task.FromResult(new ReceiveMessageResponse
            {
                HttpStatusCode = httpStatusCode,
                Messages = messages ?? new List<Message>()
            });
        }
    }
}
