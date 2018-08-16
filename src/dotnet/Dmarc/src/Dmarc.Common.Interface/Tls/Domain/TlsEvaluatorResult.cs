namespace Dmarc.Common.Interface.Tls.Domain
{
    public enum EvaluatorResult { UNKNOWN = -1, PASS = 0, PENDING = 1, INCONCLUSIVE = 2, WARNING = 3, FAIL = 4 }

    public class TlsEvaluatorResult
    {
        public TlsEvaluatorResult(EvaluatorResult? result = null, string description = null)
        {
            Result = result;
            Description = description;
        }

        public string Description { get; }

        public EvaluatorResult? Result { get; }
    }
}
