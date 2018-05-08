namespace Dmarc.MxSecurityTester.Config
{
    internal interface IMxSecurityTesterConfig
    {
        int RefreshIntervalSeconds { get; }
        int FailureRefreshIntervalSeconds { get; }
        int DomainLimit { get; }
        int SchedulerRunIntervalSeconds { get; }
        string SmtpHostName { get; }
        bool CachingEnabled { get; }
    }
}