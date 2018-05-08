namespace Dmarc.Common.Interface.Tls.Domain
{
    public enum EvaluatorResult { PASS = 0, WARNING = 1, FAIL = 2, INCONCLUSIVE = 3, PENDING = 4 }

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
