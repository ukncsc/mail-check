using System.Collections.Generic;

namespace Dmarc.MxSecurityEvaluator.Domain
{
    public interface IMxSecurityEvaluatorArguments
    {
        List<int> DomainIds { get; }
    }

    public class MxSecurityEvaluatorArguments : IMxSecurityEvaluatorArguments
    {
        public List<int> DomainIds { get; }

        public MxSecurityEvaluatorArguments(List<int> domainIds)
        {
            DomainIds = domainIds;
        }
    }
}
