using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dmarc.AggregateReport.Api.Messages
{
    internal class ValidataionErrorResponse : ErrorResponse
    {
        public ValidataionErrorResponse(string errorMessage, List<string> validationErrors) 
            : base(errorMessage)
        {
            ValidationErrors = validationErrors;
        }

        [JsonProperty(PropertyName = "validationErrors")]
        public List<string> ValidationErrors { get; }
    }
}