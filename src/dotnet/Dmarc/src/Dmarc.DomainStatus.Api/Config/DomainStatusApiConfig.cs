using System;

namespace Dmarc.DomainStatus.Api.Config
{
    public interface IDomainStatusApiConfig
    {
        string CertificateEvaluatorApiEndpoint { get; }
        string ReverseDnsApiEndpoint { get; }
    }

    public class DomainStatusApiConfig : IDomainStatusApiConfig
    {
        public string CertificateEvaluatorApiEndpoint => Environment.GetEnvironmentVariable("CertificateEvaluatorApiEndpoint");

        public string ReverseDnsApiEndpoint => Environment.GetEnvironmentVariable("ReverseDnsApiEndpoint");
    }
}
