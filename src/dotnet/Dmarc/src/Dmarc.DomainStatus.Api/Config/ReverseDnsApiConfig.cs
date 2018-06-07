using System;

namespace Dmarc.DomainStatus.Api.Config
{
    public interface IReverseDnsApiConfig
    {
        string Endpoint { get; }
    }

    public class ReverseDnsApiConfig : IReverseDnsApiConfig
    {
        public string Endpoint => Environment.GetEnvironmentVariable("ReverseDnsApiEndpoint");
    }
}
