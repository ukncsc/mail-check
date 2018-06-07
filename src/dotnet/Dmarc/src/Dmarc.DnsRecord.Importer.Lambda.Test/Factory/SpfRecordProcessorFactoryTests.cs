using System;
using Dmarc.Common.Interface.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Factory;
using Dmarc.DnsRecord.Importer.Lambda.RecordProcessor;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Importer.Lambda.Test.Factory
{
    [TestFixture]
    public class SpfRecordProcessorFactoryTests
    {
        [Test]
        public void SpfRecordProcessorCorrectedCreated()
        {
            Environment.SetEnvironmentVariable("DnsRecordLimit", "50");
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "50");
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "50");
            Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", "50");
            Environment.SetEnvironmentVariable("RefreshIntervalSeconds", "50");
            Environment.SetEnvironmentVariable("FailureRefreshIntervalSeconds", "50");
            Environment.SetEnvironmentVariable("RemainingTimeThresholdSeconds", "50");
            Environment.SetEnvironmentVariable("SnsTopicArn", "http://test.topic");
            Environment.SetEnvironmentVariable("ConnectionString", "ConnectionString");

            IDnsRecordProcessor recordProcessor = SpfRecordProcessorFactory.Create(A.Fake<ILogger>());
            Assert.That(recordProcessor, Is.Not.Null);
        }
    }
}
