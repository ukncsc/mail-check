using System;
using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Interface.Tls;
using Dmarc.MxSecurityTester.Caching;

namespace Dmarc.MxSecurityTester.Config
{
    internal class MxSecurityTesterConfig : IMxSecurityTesterConfig, IRedisConfig, IPublisherConfig, ITlsClientConfig
    {
        public MxSecurityTesterConfig(IEnvironmentVariables environmentVariables)
        {
            DomainLimit = environmentVariables.GetAsInt("MxRecordLimit");
            RefreshIntervalSeconds = environmentVariables.GetAsInt("RefreshIntervalSeconds");
            FailureRefreshIntervalSeconds = environmentVariables.GetAsInt("FailureRefreshIntervalSeconds");
            TlsConnectionTimeOut = TimeSpan.FromSeconds(environmentVariables.GetAsInt("TlsTestTimeoutSeconds"));
            SchedulerRunIntervalSeconds = environmentVariables.GetAsInt("SchedulerRunIntervalSeconds");
            SmtpHostName = environmentVariables.Get("SmtpHostName");
            CacheHostName = environmentVariables.Get("CacheHostName");
            CachingEnabled = environmentVariables.GetAsBoolOrDefault("CachingEnabled", true);
            PublisherConnectionString = environmentVariables.Get("SnsTopicArn");
        }

        public int RefreshIntervalSeconds { get; }
        public int FailureRefreshIntervalSeconds { get; }
        public int DomainLimit { get; }
        public int SchedulerRunIntervalSeconds { get; }
        public string SmtpHostName { get; }
        public string CacheHostName { get; }
        public bool CachingEnabled { get; }
        public TimeSpan TlsConnectionTimeOut { get; }
        public string PublisherConnectionString { get; }
    }
}
