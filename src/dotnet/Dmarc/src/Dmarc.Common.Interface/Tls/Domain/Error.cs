namespace Dmarc.Common.Interface.Tls.Domain
{
    public enum Error : byte
    {
        CLOSE_NOTIFY                    = 0,
        UNEXPECTED_MESSAGE              = 10,
        BAD_RECORD_MAC                  = 20,
        DECRYPTION_FAILED               = 21,
        RECORD_OVERFLOW                 = 22,
        DECOMPRESSION_FAILURE           = 30,
        HANDSHAKE_FAILURE               = 40,
        NO_CERTIFICATE                  = 41,
        BAD_CERTIFICATE                 = 42,
        UNSUPPORTED_CERTIFICATE         = 43,
        CERTIFICATE_REVOKED             = 44,
        CERTIFICATE_EXPIRED             = 45,
        CERTIFICATE_UNKNOWN             = 46,
        ILLEGAL_PARAMETER               = 47,
        UNKNOWN_CA                      = 48,
        ACCESS_DENIED                   = 49,
        DECODE_ERROR                    = 50,
        DECRYPT_ERROR                   = 51,
        EXPORT_RESTRICTION              = 60,
        PROTOCOL_VERSION                = 70,
        INSUFFICIENT_SECURITY           = 71,
        INTERNAL_ERROR                  = 80,
        INAPPROPRIATE_FALLBACK          = 86,
        USER_CANCELED                   = 90,
        NO_RENEGOTIATION                = 100,
        UNSUPPORTED_EXTENSION           = 110,
        CERTIFICATE_UNOBTAINABLE        = 111,
        UNRECOGNIZED_NAME               = 112,
        BAD_CERTIFICATE_STATUS_RESPONSE = 113,
        BAD_CERTIFICATE_HASH_VALUE      = 114,
        UNKNOWN_PSK_IDENTITY            = 115,

        NO_APPLICATION_PROTOCOL         = 120,

        TCP_CONNECTION_FAILED           = 200,
        SESSION_INITIALIZATION_FAILED   = 201,
        HOST_NOT_FOUND                  = 202
    };
}