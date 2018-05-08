using System;
using Dmarc.Common.Interface.Tls.Domain;
using Org.BouncyCastle.Crypto.Tls;

namespace Dmarc.Common.Tls.BouncyCastle.Mapping
{
    public static class BouncyCastleMappingExtensionMethods
    {
        public static ProtocolVersion ToProtocolVersion(this TlsVersion version)
        {
            switch (version)
            {
                case TlsVersion.SslV3:
                    return ProtocolVersion.SSLv3;
                case TlsVersion.TlsV1:
                    return ProtocolVersion.TLSv10;
                case TlsVersion.TlsV11:
                    return ProtocolVersion.TLSv11;
                case TlsVersion.TlsV12:
                    return ProtocolVersion.TLSv12;
                default:
                    throw new InvalidOperationException($"Cannot convert ({version}) to BouncyCastle ProtocolVersion.");
            }
        }
    }
}
