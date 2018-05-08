using Dmarc.AggregateReport.Parser.Lambda.Domain;

namespace Dmarc.AggregateReport.Parser.Lambda.Persistence.Single
{
    public interface IAttachmentPersitor
    {
        void Persist(AttachmentInfo attachment);
    }
}
