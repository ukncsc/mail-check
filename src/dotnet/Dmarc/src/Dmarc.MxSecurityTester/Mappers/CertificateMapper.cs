using System.Security.Cryptography.X509Certificates;
using Dmarc.MxSecurityTester.Dao.Entities;

namespace Dmarc.MxSecurityTester.Mappers
{
    public static class CertificateMapper
    {
        public static Certificate MapCertificate(this X509Certificate2 x509Certificate2, bool valid)
        {
            int keyLength = x509Certificate2.PublicKey.Oid.FriendlyName == "RSA"
                ? x509Certificate2.GetRSAPublicKey().KeySize
                : x509Certificate2.GetECDsaPublicKey().KeySize;

            return new Certificate(
                x509Certificate2.Thumbprint,
                x509Certificate2.Issuer,
                x509Certificate2.Subject,
                x509Certificate2.NotBefore.ToUniversalTime(),
                x509Certificate2.NotAfter.ToUniversalTime(),
                keyLength,
                x509Certificate2.PublicKey.Oid.FriendlyName,
                x509Certificate2.SerialNumber,
                x509Certificate2.Version,
                valid
            );
        }
    }
}