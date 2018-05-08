using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dmarc.DnsRecord.Evaluator.Spf.ReadModel
{
    public class Error
    {
        public Error(ErrorType errorType, string errorScope, string message)
        {
            ErrorType = errorType;
            ErrorScope = errorScope;
            Message = message;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorType ErrorType { get; }
        public string ErrorScope { get; }
        public string Message { get; }
    }
}