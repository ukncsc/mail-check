using System;

namespace Dmarc.Common.Interface.Tls
{
    public interface ITlsClientConfig
    {
        TimeSpan TlsConnectionTimeOut { get; }
    }
}