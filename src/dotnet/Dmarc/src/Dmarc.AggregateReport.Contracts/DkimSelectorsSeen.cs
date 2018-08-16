using System.Collections.Generic;
using Dmarc.Common.Interface;

namespace Dmarc.AggregateReport.Contracts
{
    public class DkimSelectorsSeen : Message
    {
        public DkimSelectorsSeen(string correlationId, string causationId,
            string id, List<string> selectors) 
            : base(id)
        {
            CorrelationId = correlationId;
            CausationId = causationId;
            Selectors = selectors;
        }

        public List<string> Selectors { get; }
    }
}