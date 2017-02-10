using System.Collections.Generic;
using System.Linq;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Persistence.Single;
using Dmarc.Common.Linq;

namespace Dmarc.AggregateReport.Parser.Common.Persistence.Multi
{
    public interface IMultiAttachmentPersistor : IAttachmentPersitor
    {
        int Count { get; }

        bool Active { get; }
    }

    public class MultiAttachmentPersistor : IMultiAttachmentPersistor
    {
        private readonly IEnumerable<IAttachmentPersitor> _persistors;

        public MultiAttachmentPersistor(IEnumerable<IAttachmentPersitor> persistors)
        {
            _persistors = persistors;
        }

        public void Persist(IEnumerable<AttachmentInfo> attachments)
        {
            _persistors.ForEach(_ => _.Persist(attachments));
        }

        public int Count => _persistors.Count();

        public bool Active => _persistors.Any();
    }
}