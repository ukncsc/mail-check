using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public interface IQualifierExplainer
    {
        string Explain(Qualifier qualifier, bool lowerCase = false);
    }

    public class QualifierExplainer : IQualifierExplainer
    {
        public string Explain(Qualifier qualifier, bool lowerCase = false)
        {
            return lowerCase
                ? Explain(qualifier).ToLower()
                : Explain(qualifier);
        }

        private static string Explain(Qualifier qualifier)
        {
            switch (qualifier)
            {
                case Qualifier.Pass:
                    return SpfExplainerResource.QualifierPassExplanation;
                case Qualifier.Fail:
                    return SpfExplainerResource.QualifierFailExplanation;
                case Qualifier.SoftFail:
                    return SpfExplainerResource.QualifierSoftFailExplanation;
                case Qualifier.Neutral:
                    return SpfExplainerResource.QualifierNeutralExplanation;
                default:
                    return null;
            }
        }
    }
}