using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Config;
using Dmarc.AggregateReport.Parser.Lambda.Email;
using Dmarc.Common.Logging;
using Dmarc.Lambda.AggregateReport.Parser.Test.Util;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Email
{
    [TestFixture]
    public class S3EmailMessageClientTests
    {
        private S3EmailMessageClient _s3EmailMessageClient;
        private IAmazonS3 _s3Client;
        private ILogger _log;
        private ILambdaAggregateReportParserConfig _lambdaAggregateReportParserConfig;

        [SetUp]
        public void SetUp()
        {
            _lambdaAggregateReportParserConfig = A.Fake<ILambdaAggregateReportParserConfig>();
            _s3Client = A.Fake<IAmazonS3>();
            _log = A.Fake<ILogger>();
            _s3EmailMessageClient = new S3EmailMessageClient(_lambdaAggregateReportParserConfig, _s3Client, _log);
        }

        [Test]
        public async Task CreateEmailMessageThrowsIfCallToS3Timesout()
        {
            A.CallTo(() => _lambdaAggregateReportParserConfig.TimeoutS3).Returns(TimeSpan.Zero);

            A.CallTo(() => _s3Client.GetObjectAsync(A<string>._, A<string>._, A<CancellationToken>._)).Returns(new TaskCompletionSource<GetObjectResponse>().Task);

            S3Event s3Event = CreateS3Event();

            await AssertEx.ThrowsAsync<TimeoutException>(() => _s3EmailMessageClient.GetEmailMessages("TestRequestId", s3Event));
        }

        [Test]
        public async Task CreateEmailMessageReturnsEmailMessage()
        {
            A.CallTo(() => _lambdaAggregateReportParserConfig.TimeoutS3).Returns(TimeSpan.FromSeconds(1));

            A.CallTo(() => _s3Client.GetObjectAsync(A<string>._, A<string>._, A<CancellationToken>._)).Returns(Task.FromResult(new GetObjectResponse { ResponseStream = Stream.Null }));

            S3Event s3Event = CreateS3Event();
            EmailMessageInfo[] emailMessageInfos = await _s3EmailMessageClient.GetEmailMessages("TestRequestId", s3Event);

            Assert.That(emailMessageInfos.Length, Is.EqualTo(1));
            Assert.That(emailMessageInfos.First(), Is.Not.Null);
        }

        private static S3Event CreateS3Event()
        {
            var s3Event = new S3Event
            {
                Records = new List<S3EventNotification.S3EventNotificationRecord>
                {
                    new S3EventNotification.S3EventNotificationRecord
                    {
                        S3 = new S3EventNotification.S3Entity
                        {
                            Bucket = new S3EventNotification.S3BucketEntity
                            {
                                Name = "TestBucketName"
                            },
                            Object = new S3EventNotification.S3ObjectEntity
                            {
                                Key = "TestKey"
                            }
                        }
                    }
                }
            };
            return s3Event;
        }
    }
}
