using System;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Explainers
{
    public class FailureOptionsExplainer : BaseTagExplainerStrategy<FailureOption>
    {
        public override string GetExplanation(FailureOption tConcrete)
        {
            switch (tConcrete.FailureOptionType)
            {
                case FailureOptionType.Zero:
                    return DmarcExplainerResource.FailureOptionsZeroExplanation;
                case FailureOptionType.One:
                    return DmarcExplainerResource.FailureOptionsOneExplanation;
                case FailureOptionType.D:
                    return DmarcExplainerResource.FailureOptionsDExplanation;
                case FailureOptionType.S:
                    return DmarcExplainerResource.FailureOptionsSExplanation;
                default:
                    throw new ArgumentException($"Unexpected {nameof(FailureOptionType)}: {tConcrete.FailureOptionType}");
            }
        }
    }
}