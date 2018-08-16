using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dmarc.Common.Api.Domain
{
    public enum ErrorStatus
    {
        Information,
        Warning,
        Error
    }

    public class ErrorResponse
    {
        public ErrorResponse(string message, ErrorStatus status = ErrorStatus.Error)
        {
            Message = message;
            Status = status;
        }

        public string Message { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorStatus Status { get; }
    }
}
