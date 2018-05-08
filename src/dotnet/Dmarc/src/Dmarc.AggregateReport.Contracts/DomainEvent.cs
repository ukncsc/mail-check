using System;

namespace Dmarc.AggregateReport.Contracts
{
    public abstract class DomainEvent
    {
        protected DomainEvent(string correlationId, string causationId)
        {
            MessageId = Guid.NewGuid().ToString();
            CorrelationId = correlationId;
            CausationId = causationId;
        }

        public string CorrelationId { get; }
        public string CausationId { get; }
        public string MessageId { get; }
    }
}