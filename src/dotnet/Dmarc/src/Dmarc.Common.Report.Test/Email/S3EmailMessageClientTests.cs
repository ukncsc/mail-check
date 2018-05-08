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
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Config;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Report.Email;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Common.Report.Test.Email
{
    [TestFixture]
    public class S3EmailMessageClientTests
    {
        private S3EmailMessageClient _s3EmailMessageClient;
        private IAmazonS3 _s3Client;
        private ILogger _log;
        private ILambdaReportParserConfig _lambdaReportParserConfig;

        [SetUp]
        public void SetUp()
        {
            _lambdaReportParserConfig = A.Fake<ILambdaReportParserConfig>();
            _s3Client = A.Fake<IAmazonS3>();
            _log = A.Fake<ILogger>();
            _s3EmailMessageClient = new S3EmailMessageClient(_lambdaReportParserConfig, _s3Client, _log);
        }

        [Test]
        public void CreateEmailMessageThrowsIfCallToS3Timesout()
        {
            A.CallTo(() => _lambdaReportParserConfig.TimeoutS3).Returns(TimeSpan.Zero);

            A.CallTo(() => _s3Client.GetObjectAsync(A<string>._, A<string>._, A<CancellationToken>._)).Returns(new TaskCompletionSource<GetObjectResponse>().Task);

            S3Event s3Event = CreateS3Event();

            Assert.ThrowsAsync<TimeoutException>(async () => await _s3EmailMessageClient.GetEmailMessages("TestRequestId", "TestMessageId", s3Event));
        }

        [Test]
        public async Task CreateEmailMessageReturnsEmailMessage()
        {
            A.CallTo(() => _lambdaReportParserConfig.TimeoutS3).Returns(TimeSpan.FromSeconds(1));

            A.CallTo(() => _s3Client.GetObjectAsync(A<string>._, A<string>._, A<CancellationToken>._)).Returns(Task.FromResult(new GetObjectResponse { ResponseStream = Stream.Null }));

            S3Event s3Event = CreateS3Event();
            EmailMessageInfo[] emailMessageInfos = await _s3EmailMessageClient.GetEmailMessages("TestRequestId", "TestMessageId", s3Event);

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
