namespace Dmarc.DnsRecord.Evaluator.Rules
{
    public class Error
    {
        public Error(ErrorType errorType, string message)
        {
            ErrorType = errorType;
            Message = message;
        }

        public ErrorType ErrorType { get; }

        public string Message { get; }

        public override string ToString()
        {
            return $"{nameof(ErrorType)}: {ErrorType}, {nameof(Message)}: {Message}";
        }
    }
}