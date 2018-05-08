using Org.BouncyCastle.Crypto.Tls;

namespace Dmarc.Common.Tls.BouncyCastle
{
    internal class BasicTlsClient : DefaultTlsClient
    {
        public override TlsAuthentication GetAuthentication()
        {
            return new EmptyTlsAuthentication();
        }
    }
}