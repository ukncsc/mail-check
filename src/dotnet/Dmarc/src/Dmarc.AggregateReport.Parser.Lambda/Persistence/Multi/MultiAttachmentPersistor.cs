using System.Collections.Generic;
using System.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Persistence.Single;
using Dmarc.Common.Linq;

namespace Dmarc.AggregateReport.Parser.Lambda.Persistence.Multi
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

        public void Persist(AttachmentInfo attachment)
        {
            _persistors.ForEach(_ => _.Persist(attachment));
        }

        public int Count => _persistors.Count();

        public bool Active => _persistors.Any();
    }
}