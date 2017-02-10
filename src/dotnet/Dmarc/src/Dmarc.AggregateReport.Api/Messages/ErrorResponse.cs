using Newtonsoft.Json;

namespace Dmarc.AggregateReport.Api.Messages
{
    internal class ErrorResponse : Response
    {
        public ErrorResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        [JsonProperty(PropertyName = "errorMessage")]
        public string ErrorMessage { get; }
    }
}