using System;
using Dmarc.Common.Interface;

namespace Dmarc.Admin.Api.Contract.Messages
{
    public class DomainCreated : Message
    {
        public DomainCreated(string id, string createdBy, DateTime creationDate)
            : base(id)
        {
            CausationId = null;
            CorrelationId = Guid.NewGuid().ToString();
            CreatedBy = createdBy;
            CreationDate = creationDate;
        }

        public string CreatedBy { get; }

        public DateTime CreationDate { get; }
    }
}
