using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Config;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Report.Email;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Common.Report.Test.Email
{
    [TestFixture]
    public class S3EmailMessageProcessorTests
    {
        private ILambdaReportParserConfig _lambdaReportParseConfig;
        private S3EmailMessageProcessor<Domain> _s3EmailMessageProcessor;
        private IS3EmailMessageClient _emailMessageClient;
        private ILogger _log;
        private IEmailMessageInfoProcessor<Domain> _emailMessageInfoProcessor;

        [SetUp]
        public void SetUp()
        {
            _emailMessageClient = A.Fake<IS3EmailMessageClient>();
            _lambdaReportParseConfig = A.Fake<ILambdaReportParserConfig>();
            _emailMessageInfoProcessor = A.Fake<IEmailMessageInfoProcessor<Domain>>();
            _log = A.Fake<ILogger>();
            
            _s3EmailMessageProcessor = new S3EmailMessageProcessor<Domain>(
                _emailMessageClient,
                _emailMessageInfoProcessor,
                _lambdaReportParseConfig,
                _log);
        }

        [Test]
        public async Task ProcessMailMessageMessageIsProcessed()
        {
            A.CallTo(() => _lambdaReportParseConfig.MaxS3ObjectSizeKilobytes).Returns(2);

            A.CallTo(() => _emailMessageClient.GetEmailMessages(A<string>._, A<string>._, A<S3Event>._)).Returns(Task.FromResult(new[]
            {
                new EmailMessageInfo(new EmailMetadata("uri", "filename", 1), Stream.Null)
            }));

            A.CallTo(() => _emailMessageInfoProcessor.ProcessEmailMessage(A<EmailMessageInfo>._))
                .Returns(new Result<Domain>(new Domain(), true, false));

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            S3Event s3Event = new S3Event();
            bool result = await _s3EmailMessageProcessor.ProcessEmailMessage(string.Empty, lambdaContext, s3Event);

            Assert.That(result, Is.True);
            A.CallTo(() => _log.Warn(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _emailMessageInfoProcessor.ProcessEmailMessage(A<EmailMessageInfo>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task ProcessMailMessageMessageExceedsSizeThresholdMessageIsNotParsed()
        {
            A.CallTo(() => _lambdaReportParseConfig.MaxS3ObjectSizeKilobytes).Returns(1);

            A.CallTo(() => _emailMessageClient.GetEmailMessages(A<string>._, A<string>._, A<S3Event>._)).Returns(Task.FromResult(new[]
            {
                new EmailMessageInfo(new EmailMetadata("uri", "filename", 2), Stream.Null)
            }));

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            S3Event s3Event = new S3Event();
            bool result = await _s3EmailMessageProcessor.ProcessEmailMessage(string.Empty, lambdaContext, s3Event);

            Assert.That(result, Is.False);
            A.CallTo(() => _log.Warn(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _emailMessageInfoProcessor.ProcessEmailMessage(A<EmailMessageInfo>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task ReturnFalseIfNoResults()
        {
            A.CallTo(() => _lambdaReportParseConfig.MaxS3ObjectSizeKilobytes).Returns(2);

            A.CallTo(() => _emailMessageClient.GetEmailMessages(A<string>._, A<string>._, A<S3Event>._)).Returns(Task.FromResult(new EmailMessageInfo[0]));

            A.CallTo(() => _emailMessageInfoProcessor.ProcessEmailMessage(A<EmailMessageInfo>._))
                .Returns(Result<Domain>.FailedResult);

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            S3Event s3Event = new S3Event();
            bool result = await _s3EmailMessageProcessor.ProcessEmailMessage(string.Empty, lambdaContext, s3Event);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ReturnFalseIfAnyResultFalseResults()
        {
            A.CallTo(() => _lambdaReportParseConfig.MaxS3ObjectSizeKilobytes).Returns(2);

            A.CallTo(() => _emailMessageClient.GetEmailMessages(A<string>._, A<string>._, A<S3Event>._)).Returns(Task.FromResult(new[]
            {
                new EmailMessageInfo(new EmailMetadata("uri", "filename", 1), Stream.Null),
                new EmailMessageInfo(new EmailMetadata("uri", "filename", 1), Stream.Null)
            }));

            A.CallTo(() => _emailMessageInfoProcessor.ProcessEmailMessage(A<EmailMessageInfo>._))
                .ReturnsNextFromSequence(new Result<Domain>(new Domain(), true, false),
                    Result<Domain>.FailedResult);

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            S3Event s3Event = new S3Event();
            bool result = await _s3EmailMessageProcessor.ProcessEmailMessage(string.Empty, lambdaContext, s3Event);

            Assert.That(result, Is.False);
        }
    }

    public class Domain { }

    public class Entity { }
}
