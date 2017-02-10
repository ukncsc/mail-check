using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Parsers;
using Dmarc.AggregateReport.Parser.Lambda.Config;
using Dmarc.AggregateReport.Parser.Lambda.Email;
using Dmarc.Common.Logging;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Email
{
    [TestFixture]
    public class S3EmailMessageProcessorTests
    {
        private ILambdaAggregateReportParserConfig _lambdaAggregateReportParseConfig;
        private S3EmailMessageProcessor _s3EmailMessageProcessor;
        private IS3EmailMessageClient _emailMessageClient;
        private IAggregateReportParserAsync _aggegateReportParser;
        private ILogger _log;

        [SetUp]
        public void SetUp()
        {
            _lambdaAggregateReportParseConfig = A.Fake<ILambdaAggregateReportParserConfig>();
            _emailMessageClient = A.Fake<IS3EmailMessageClient>();
            _aggegateReportParser = A.Fake<IAggregateReportParserAsync>();
            _log = A.Fake<ILogger>();
            _s3EmailMessageProcessor = new S3EmailMessageProcessor(_lambdaAggregateReportParseConfig, _emailMessageClient, _aggegateReportParser, _log);
        }

        [Test]
        public async Task ProcessMailMessageMessageIsParsed()
        {
            A.CallTo(() => _lambdaAggregateReportParseConfig.MaxS3ObjectSizeKilobytes).Returns(2);

            A.CallTo(() => _emailMessageClient.GetEmailMessages(A<string>._, A<S3Event>._)).Returns(Task.FromResult(new[]
            {
                new EmailMessageInfo(new EmailMetadata("uri", "filename", 1), Stream.Null)
            }));

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            S3Event s3Event = new S3Event();
            await _s3EmailMessageProcessor.ProcessEmailMessage(lambdaContext, s3Event);

            A.CallTo(() => _log.Warn(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _aggegateReportParser.Parse(A<EmailMessageInfo>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task ProcessMailMessageMessageExceedsSizeThresholdMessageIsNotParsed()
        {
            A.CallTo(() => _lambdaAggregateReportParseConfig.MaxS3ObjectSizeKilobytes).Returns(1);

            A.CallTo(() => _emailMessageClient.GetEmailMessages(A<string>._, A<S3Event>._)).Returns(Task.FromResult(new[]
            {
                new EmailMessageInfo(new EmailMetadata("uri", "filename", 2), Stream.Null)
            }));

            ILambdaContext lambdaContext = A.Fake<ILambdaContext>();
            S3Event s3Event = new S3Event();
            await _s3EmailMessageProcessor.ProcessEmailMessage(lambdaContext, s3Event);

            A.CallTo(() => _log.Warn(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _aggegateReportParser.Parse(A<EmailMessageInfo>._)).MustNotHaveHappened();
        }
    }
}
