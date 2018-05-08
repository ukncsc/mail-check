using Org.BouncyCastle.Crypto.Tls;

namespace Dmarc.Common.Tls.BouncyCastle
{
    internal class EmptyTlsAuthentication : TlsAuthentication
    {
        public TlsCredentials GetClientCredentials(CertificateRequest certificateRequest)
        {
            return null;
        }

        public void NotifyServerCertificate(Certificate serverCertificate)
        {
        }
    }
}