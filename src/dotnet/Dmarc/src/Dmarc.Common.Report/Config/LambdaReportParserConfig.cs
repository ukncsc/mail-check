using System;
using Dmarc.Common.Data;
using Dmarc.Common.Environment;

namespace Dmarc.Common.Report.Config
{
    public interface ILambdaReportParserConfig
    {
        string SqsQueueUrl { get; }
        TimeSpan RemainingTimeTheshold { get; }
        TimeSpan TimeoutSqs { get; }
        TimeSpan TimeoutS3 { get; }
        long MaxS3ObjectSizeKilobytes { get; }
    }

    public class LambdaReportParserConfig : ILambdaReportParserConfig, IConnectionInfo
    {
        public LambdaReportParserConfig(IEnvironmentVariables environmentVariables)
        {
            RemainingTimeTheshold = TimeSpan.FromSeconds(environmentVariables.GetAsDouble("RemainingTimeThresholdSeconds"));
            SqsQueueUrl = environmentVariables.Get("QueueUrl");
            TimeoutSqs = TimeSpan.FromSeconds(environmentVariables.GetAsDouble("TimeoutS3Seconds"));
            TimeoutS3 = TimeSpan.FromSeconds(environmentVariables.GetAsDouble("TimeoutSqsSeconds"));
            MaxS3ObjectSizeKilobytes = environmentVariables.GetAsLong("MaxS3ObjectSizeKilobytes");
            ConnectionString = environmentVariables.Get("ConnectionString");
        }

        public string SqsQueueUrl { get; }
        public TimeSpan RemainingTimeTheshold { get; }
        public TimeSpan TimeoutSqs { get; }
        public TimeSpan TimeoutS3 { get; }
        public long MaxS3ObjectSizeKilobytes { get; }
        public string ConnectionString { get; }
    }
}
