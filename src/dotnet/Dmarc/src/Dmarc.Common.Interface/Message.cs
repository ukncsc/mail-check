using System;

namespace Dmarc.Common.Interface
{
    public abstract class Message
    {
        protected Message(string id)
        {
            Id = id;    //id of the entity that this message corresponds to e.g. domain name.
        }

        public string Id { get; }
        public string CorrelationId { get; set; } //will be set by framework on out going messages.
        public string CausationId { get; set; } // will be set by framework on out going messages.
        public string MessageId { get; set; } //this is set by the framework on incoming messages to sns/sqs messageId.
        public DateTime Timestamp { get; set; } //this is set by the framework on incoming messages to sns/sqs messageId.
    }
}
