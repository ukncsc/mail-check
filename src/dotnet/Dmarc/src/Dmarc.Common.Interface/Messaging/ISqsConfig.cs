namespace Dmarc.Common.Interface.Messaging
{
    public interface ISqsConfig
    {
        string QueueUrl { get; }
        int MaxNumberOfMessages { get; }
        int WaitTimeSeconds { get; }
    }
}
