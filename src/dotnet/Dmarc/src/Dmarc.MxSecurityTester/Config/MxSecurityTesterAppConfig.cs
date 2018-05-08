using System;
using Dmarc.Common.Interface.Tls;
using Dmarc.MxSecurityTester.Caching;

namespace Dmarc.MxSecurityTester.Config
{
    internal class MxSecurityTesterAppConfig : IMxSecurityTesterConfig, IRedisConfig, ITlsClientConfig
    {
        public MxSecurityTesterAppConfig()
        {
            DomainLimit = 0;
            RefreshIntervalSeconds = 0;
            FailureRefreshIntervalSeconds = 0;
            TlsConnectionTimeOut = TimeSpan.FromSeconds(5);
            SchedulerRunIntervalSeconds = 0;
            SmtpHostName = "localhost";
            CacheHostName = string.Empty;
            CachingEnabled = false;
        }

        public int RefreshIntervalSeconds { get; }
        public int FailureRefreshIntervalSeconds { get; }
        public int DomainLimit { get; }
        public int SchedulerRunIntervalSeconds { get; }
        public string SmtpHostName { get; }
        public string CacheHostName { get; }
        public bool CachingEnabled { get; }
        public TimeSpan TlsConnectionTimeOut { get; }
    }
}