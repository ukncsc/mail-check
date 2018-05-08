using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Messaging;

namespace Dmarc.MxSecurityEvaluator.Config
{
    public class MxSecurityEvaluatorSqsConfig : ISqsConfig
    {
        public MxSecurityEvaluatorSqsConfig(IEnvironmentVariables environmentVariables)
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
