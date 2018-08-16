using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class CertificateEvaluatorApiResponse
    {
        public string DomainName { get; set; }

        public List<HostResult> HostResults { get; set; }
    }

    public class HostResult
    {
        public string HostName { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorType ErrorType { get; set; }
    }

    public enum ErrorType
    {
        Inconclusive = 3,
        Warning = 4,
        Error = 5
    }
}
