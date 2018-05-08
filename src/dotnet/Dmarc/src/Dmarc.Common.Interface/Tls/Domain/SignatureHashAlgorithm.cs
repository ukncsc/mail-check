namespace Dmarc.Common.Interface.Tls.Domain
{
    public enum SignatureHashAlgorithm : ushort
    {
        SHA512_RSA = 0x0601,
        SHA512_DSA = 0x0602,
        SHA512_ECDSA = 0x0603,
        SHA384_RSA = 0x0501,
        SHA384_DSA = 0x0502,
        SHA384_ECDSA = 0x0503,
        SHA256_RSA = 0x0401,
        SHA256_DSA = 0x0402,
        SHA256_ECDSA = 0x0403,
        SHA224_RSA = 0x0301,
        SHA224_DSA = 0x0302,
        SHA224_ECDSA = 0x0303,
        SHA1_RSA = 0x0201,
        SHA1_DSA = 0x0202,
        SHA1_ECDSA = 0x0203,

        UNKNOWN = 0xffff
    }
}