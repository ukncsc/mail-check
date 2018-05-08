using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Dmarc.Common.Interface.Tls.Domain;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tls;
using CipherSuite = Dmarc.Common.Interface.Tls.Domain.CipherSuite;

namespace Dmarc.Common.Tls.BouncyCastle.Mapping
{
    public static class DomainMappingExtensionMethods
    {
        private static readonly Dictionary<string, CurveGroup> CurveLookup = new Dictionary<string, CurveGroup>
        {
            {"secp160k1curve", CurveGroup.Secp160k1},
            {"secp160r1curve", CurveGroup.Secp160r1},
            {"secp160r2curve", CurveGroup.Secp160r2},
            {"secp192k1curve", CurveGroup.Secp192k1},
            {"secp192r1curve", CurveGroup.Secp192r1},
            {"secp224k1curve", CurveGroup.Secp224k1},
            {"secp224r1curve", CurveGroup.Secp224r1},
            {"secp256k1curve", CurveGroup.Secp256k1},
            {"secp256r1curve", CurveGroup.Secp256r1},
            {"secp384r1curve", CurveGroup.Secp384r1},
            {"secp521r1curve", CurveGroup.Secp521r1},
            {"sect163k1curve", CurveGroup.Sect163k1},
            {"sect163r1curve", CurveGroup.Sect163r1},
            {"sect163r2curve", CurveGroup.Sect163r2},
            {"sect193r1curve", CurveGroup.Sect193r1},
            {"sect193r2curve", CurveGroup.Sect193r2},
            {"sect233k1curve", CurveGroup.Sect233k1},
            {"sect233r1curve", CurveGroup.Sect233r1},
            {"sect239k1curve", CurveGroup.Sect239k1},
            {"sect283k1curve", CurveGroup.Sect283k1},
            {"sect283r1curve", CurveGroup.Sect283r1},
            {"sect409k1curve", CurveGroup.Sect409k1},
            {"sect409r1curve", CurveGroup.Sect409r1},
            {"sect571k1curve", CurveGroup.Sect571k1},
            {"sect571r1curve", CurveGroup.Sect571r1},
        };

        private static readonly Dictionary<DHParameters, CurveGroup> GroupLookup = new Dictionary<DHParameters, CurveGroup>
        {

            {AdditionalGroups.Java1024, CurveGroup.Java1024},
            {DHStandardGroups.rfc2409_1024, CurveGroup.Rfc2409_1024},
            {DHStandardGroups.rfc5114_1024_160, CurveGroup.Rfc5114_1024},
            {DHStandardGroups.rfc7919_ffdhe2048, CurveGroup.Ffdhe2048},
            {DHStandardGroups.rfc7919_ffdhe3072, CurveGroup.Ffdhe3072},
            {DHStandardGroups.rfc7919_ffdhe4096, CurveGroup.Ffdhe4096},
            {DHStandardGroups.rfc7919_ffdhe6144, CurveGroup.Ffdhe6144},
            {DHStandardGroups.rfc7919_ffdhe8192, CurveGroup.Ffdhe8192},
        };

        public static CurveGroup ToCurve(this string curveName)
        {
            CurveGroup curve;
            return CurveLookup.TryGetValue(curveName, out curve) ? curve : CurveGroup.Unknown;
        }

        public static CurveGroup ToGroup(this DHParameters parameters)
        {
            CurveGroup group;
            if (GroupLookup.TryGetValue(parameters, out group))
            {
                return group;
            }

            switch (parameters.P.BitLength)
            {
                case 1024:
                    return CurveGroup.UnknownGroup1024;
                case 2048:
                    return CurveGroup.UnknownGroup2048;
                case 3072:
                    return CurveGroup.UnknownGroup3072;
                case 4096:
                    return CurveGroup.UnknownGroup4096;
                case 6144:
                    return CurveGroup.UnknownGroup6144;
                case 8192:
                    return CurveGroup.UnknownGroup8192;
            }

            return CurveGroup.Unknown;
        }

        public static TlsVersion ToTlsVersion(this ProtocolVersion protocolVersion)
        {
            switch (protocolVersion.FullVersion)
            {
                case 768:
                    return TlsVersion.SslV3;
                case 769:
                    return TlsVersion.TlsV1;
                case 770:
                    return TlsVersion.TlsV11;
                case 771:
                    return TlsVersion.TlsV12;
                default:
                    return TlsVersion.Unknown;
            }
        }

        public static CipherSuite ToCipherSuite(this int cipherSuite)
        {
            return (CipherSuite)cipherSuite;
        }

        public static SignatureHashAlgorithm ToSignatureAlgorithm(this SignatureAndHashAlgorithm signatureAndHashAlgorithm)
        {
            if (signatureAndHashAlgorithm != null)
            {
                ushort signatureHashAlg = (ushort) (signatureAndHashAlgorithm.Hash << 8 | signatureAndHashAlgorithm.Signature);
                return (SignatureHashAlgorithm) signatureHashAlg;
            }
            return SignatureHashAlgorithm.UNKNOWN;
        }

        public static List<X509Certificate2> ToCertificateList(this Org.BouncyCastle.Crypto.Tls.Certificate certificate)
        {
            return certificate.GetCertificateList().Select(_ => new X509Certificate2(_.GetDerEncoded())).ToList();
        }
    }
}
