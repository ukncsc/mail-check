using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Messaging;
using System;

namespace Dmarc.DnsRecord.Evaluator.Config
{
    public class RecordEvaluatorConfig : ISqsConfig
    {
        public RecordEvaluatorConfig(IEnvironmentVariables environmentVariables)
        {
            MaxNumberOfMessages = 1;
            QueueUrl = environmentVariables.Get("SqsQueueUrl");
            WaitTimeSeconds = 20;
        }

        public int MaxNumberOfMessages { get; }
        public string QueueUrl { get; }
        public int WaitTimeSeconds { get; }
    }
}
