namespace Dmarc.Common.Interface.Tls.Domain
{
    public enum CurveGroup : ushort
    {
        //curves
        Sect163k1 = 0x0001,
        Sect163r1 = 0x0002,
        Sect163r2 = 0x0003,
        Sect193r1 = 0x0004,
        Sect193r2 = 0x0005,
        Sect233k1 = 0x0006,
        Sect233r1 = 0x0007,
        Sect239k1 = 0x0008,
        Sect283k1 = 0x0009,
        Sect283r1 = 0x000a,
        Sect409k1 = 0x000b,
        Sect409r1 = 0x000c,
        Sect571k1 = 0x000d,
        Sect571r1 = 0x000e,
        Secp160k1 = 0x000f,
        Secp160r1 = 0x0010,
        Secp160r2 = 0x0011,
        Secp192k1 = 0x0012,
        Secp192r1 = 0x0013,
        Secp224k1 = 0x0014,
        Secp224r1 = 0x0015,
        Secp256k1 = 0x0016,
        Secp256r1 = 0x0017,
        Secp384r1 = 0x0018,
        Secp521r1 = 0x0019,

        //groups
        Ffdhe2048 = 0x0100,
        Ffdhe3072 = 0x0101,
        Ffdhe4096 = 0x0102,
        Ffdhe6144 = 0x0103,
        Ffdhe8192 = 0x0104,

        Unknown   = 0xfe00,

        UnknownGroup1024 = 0xfe01,
        UnknownGroup2048 = 0xfe02,
        UnknownGroup3072 = 0xfe03,
        UnknownGroup4096 = 0xfe04,
        UnknownGroup6144 = 0xfe05,
        UnknownGroup8192 = 0xfe06,

        Java1024 = 0xfe10,
        Rfc2409_1024 = 0xfe11,
        Rfc5114_1024 = 0xfe12,
    }
}