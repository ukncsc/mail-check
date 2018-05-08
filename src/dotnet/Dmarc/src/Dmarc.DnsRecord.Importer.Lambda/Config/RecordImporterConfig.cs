using System;
using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Messaging;

namespace Dmarc.DnsRecord.Importer.Lambda.Config
{
    public interface IRecordImporterConfig : IPublisherConfig
    {
        TimeSpan RemainingTimeTheshold { get; }
        int RefreshIntervalSeconds { get; }
        int FailureRefreshIntervalSeconds { get; }
        int DnsRecordLimit { get; }
    }

    public class RecordImporterConfig : IRecordImporterConfig
    {
        public RecordImporterConfig(IEnvironmentVariables environmentVariables)
        {
            DnsRecordLimit = environmentVariables.GetAsInt("DnsRecordLimit");
            RefreshIntervalSeconds = environmentVariables.GetAsInt("RefreshIntervalSeconds");
            FailureRefreshIntervalSeconds = environmentVariables.GetAsInt("FailureRefreshIntervalSeconds");
            RemainingTimeTheshold = TimeSpan.FromSeconds(environmentVariables.GetAsDouble("RemainingTimeThresholdSeconds"));
            PublisherConnectionString = environmentVariables.Get("SnsTopicArn");
        }

        public TimeSpan RemainingTimeTheshold { get; }
        public int RefreshIntervalSeconds { get; }
        public int DnsRecordLimit { get; }
        public int FailureRefreshIntervalSeconds { get; }
        public string PublisherConnectionString { get; }
    }
}