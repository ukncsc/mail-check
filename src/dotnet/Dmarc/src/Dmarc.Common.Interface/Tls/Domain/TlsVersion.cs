namespace Dmarc.Common.Interface.Tls.Domain
{
    public enum TlsVersion : ushort
    {
        Unknown = 0x0000,
        SslV3 = 0x0300,
        TlsV1 = 0x0301,
        TlsV11 = 0x0302,
        TlsV12 = 0x0303
    }
}