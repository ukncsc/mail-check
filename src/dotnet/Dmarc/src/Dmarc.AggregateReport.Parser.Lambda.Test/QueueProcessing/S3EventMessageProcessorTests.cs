using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3.Util;
using Amazon.SQS.Model;
using Dmarc.AggregateReport.Parser.Lambda.Email;
using Dmarc.AggregateReport.Parser.Lambda.QueueProcessing;
using Dmarc.Common.Logging;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.QueueProcessing
{
    [TestFixture]
    public class S3EventMessageProcessorTests
    {
        private S3EventMessageProcessor _s3EventMessageProcessor;
        private IS3EmailMessageProcessor _is3EmailMessageProcessor;
        private IS3EventDeserializer _s3EventDeserializer;
        private ILogger _log;

        [SetUp]
        public void SetUp()
        {
            _is3EmailMessageProcessor = A.Fake<IS3EmailMessageProcessor>();
            _s3EventDeserializer = A.Fake<IS3EventDeserializer>();
            _log = A.Fake<ILogger>();
            _s3EventMessageProcessor = new S3EventMessageProcessor(_is3EmailMessageProcessor, _s3EventDeserializer, _log);
        }

        [Test]
        public async Task TryProcessMessageEventIsNotS3EventDontProcessReturnFalse()
        {
            S3Event s3Event;
            A.CallTo(() => _s3EventDeserializer.TryDeserializeS3Event(A<string>._, out s3Event)).Returns(false)
                .AssignsOutAndRefParameters(s3Event = null);

            bool result = await _s3EventMessageProcessor.TryProcessMessage(A.Fake<ILambdaContext>(), new Message());
            A.CallTo(() => _is3EmailMessageProcessor.ProcessEmailMessage(A<ILambdaContext>._, A<S3Event>._)).MustNotHaveHappened();
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task TryProcessMessageEventIsS3EventButRecordsAreNullDontProcessMessageReturnFalse()
        {
            S3Event s3Event;
            A.CallTo(() => _s3EventDeserializer.TryDeserializeS3Event(A<string>._, out s3Event)).Returns(true)
                .AssignsOutAndRefParameters(new S3Event{Records = null});

            bool result = await _s3EventMessageProcessor.TryProcessMessage(A.Fake<ILambdaContext>(), new Message());
            A.CallTo(() => _is3EmailMessageProcessor.ProcessEmailMessage(A<ILambdaContext>._, A<S3Event>._)).MustNotHaveHappened();
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task TryProcessMessageEventIsS3EventButRecordsEmptyDontProcessMessageReturnFalse()
        {
            S3Event s3Event;
            A.CallTo(() => _s3EventDeserializer.TryDeserializeS3Event(A<string>._, out s3Event)).Returns(true)
                .AssignsOutAndRefParameters(new S3Event { Records = new List<S3EventNotification.S3EventNotificationRecord>() });

            bool result = await _s3EventMessageProcessor.TryProcessMessage(A.Fake<ILambdaContext>(), new Message());
            A.CallTo(() => _is3EmailMessageProcessor.ProcessEmailMessage(A<ILambdaContext>._, A<S3Event>._)).MustNotHaveHappened();
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task TryProcessMessageEventIsS3EventWithRecordsProcessEventReturnsTrue()
        {
            S3Event s3Event;
            A.CallTo(() => _s3EventDeserializer.TryDeserializeS3Event(A<string>._, out s3Event)).Returns(true)
                .AssignsOutAndRefParameters(new S3Event { Records = new List<S3EventNotification.S3EventNotificationRecord> {new S3EventNotification.S3EventNotificationRecord()} });

            bool result = await _s3EventMessageProcessor.TryProcessMessage(A.Fake<ILambdaContext>(), new Message());
            A.CallTo(() => _is3EmailMessageProcessor.ProcessEmailMessage(A<ILambdaContext>._, A<S3Event>._)).MustHaveHappened(Repeated.Exactly.Once);
            Assert.That(result, Is.True);
        }
    }
}
