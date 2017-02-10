using System.Collections.Generic;
using Dmarc.AggregateReport.Parser.Common.Domain;

namespace Dmarc.AggregateReport.Parser.Common.Persistence.Single
{
    public interface IAttachmentPersitor
    {
        void Persist(IEnumerable<AttachmentInfo> attachments);
    }
}
