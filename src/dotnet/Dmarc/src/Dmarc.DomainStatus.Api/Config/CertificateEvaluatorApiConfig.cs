using System;

namespace Dmarc.DomainStatus.Api.Config
{
    public interface ICertificateEvaluatorApiConfig
    {
        string Endpoint { get; }
    }

    public class CertificateEvaluatorApiConfig : ICertificateEvaluatorApiConfig
    {
        public string Endpoint => Environment.GetEnvironmentVariable("CertificateEvaluatorApiEndpoint");
    }
}
