using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3.Util;
using Amazon.SQS.Model;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Email;
using Dmarc.Common.Report.QueueProcessing;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Common.Report.Test.QueueProcessing
{
    [TestFixture]
    public class S3EventMessageProcessorTests
    {
        private S3EventMessageProcessor _s3EventMessageProcessor;
        private IS3EmailMessageProcessor _s3EmailMessageProcessor;
        private IS3EventDeserializer _s3EventDeserializer;
        private ILogger _log;

        [SetUp]
        public void SetUp()
        {
            _s3EmailMessageProcessor = A.Fake<IS3EmailMessageProcessor>();
            _s3EventDeserializer = A.Fake<IS3EventDeserializer>();
            _log = A.Fake<ILogger>();
            _s3EventMessageProcessor = new S3EventMessageProcessor(_s3EmailMessageProcessor, _s3EventDeserializer, _log);
        }

        [Test]
        public async Task TryProcessMessageEventIsNotS3EventDontProcessReturnFalse()
        {
            S3Event s3Event;
            A.CallTo(() => _s3EventDeserializer.TryDeserializeS3Event(A<string>._, out s3Event)).Returns(false)
                .AssignsOutAndRefParameters(s3Event = null);

            bool result = await _s3EventMessageProcessor.TryProcessMessage(A.Fake<ILambdaContext>(), new Message());
            A.CallTo(() => _s3EmailMessageProcessor.ProcessEmailMessage(A<string>._, A<ILambdaContext>._, A<S3Event>._)).MustNotHaveHappened();
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task TryProcessMessageEventIsS3EventWithRecordsProcessEventReturnsProcessMessageResult()
        {
            S3Event s3Event;
            A.CallTo(() => _s3EventDeserializer.TryDeserializeS3Event(A<string>._, out s3Event)).Returns(true)
                .AssignsOutAndRefParameters(new S3Event { Records = new List<S3EventNotification.S3EventNotificationRecord> {new S3EventNotification.S3EventNotificationRecord()} });

            A.CallTo(() => _s3EmailMessageProcessor.ProcessEmailMessage(A<string>._, A<ILambdaContext>._, A<S3Event>._)).Returns(Task.FromResult(true));

            bool result = await _s3EventMessageProcessor.TryProcessMessage(A.Fake<ILambdaContext>(), new Message());
            A.CallTo(() => _s3EmailMessageProcessor.ProcessEmailMessage(A<string>._, A<ILambdaContext>._, A<S3Event>._)).MustHaveHappened(Repeated.Exactly.Once);
            Assert.That(result, Is.True);
        }
    }
}
