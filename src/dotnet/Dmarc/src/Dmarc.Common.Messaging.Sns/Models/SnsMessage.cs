using System;

namespace Dmarc.Common.Messaging.Sns.Models
{
    public class SnsMessage
    {
        public SnsMessage(
            string type,
            string messageId,
            string topicArn,
            string message,
            DateTime timestamp,
            int signatureVersion,
            string signingCertUrl,
            string unsubscribeUrl)
        {
            Type = type;
            MessageId = messageId;
            TopicArn = topicArn;
            Message = message;
            Timestamp = timestamp;
            SignatureVersion = signatureVersion;
            SigningCertUrl = signingCertUrl;
            UnsubscribeUrl = unsubscribeUrl;
        }

        public string Type { get; }
        public string MessageId { get; }
        public string TopicArn { get; }
        public string Message { get; }
        public DateTime Timestamp { get; }
        public int SignatureVersion { get; }
        public string SigningCertUrl { get; }
        public string UnsubscribeUrl { get; }
    }
}
