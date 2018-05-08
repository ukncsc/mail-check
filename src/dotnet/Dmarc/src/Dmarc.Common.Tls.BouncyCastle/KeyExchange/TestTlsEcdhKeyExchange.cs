using System.Collections;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tls;

namespace Dmarc.Common.Tls.BouncyCastle.KeyExchange
{
    internal class TestTlsEcDhKeyExchange : TlsECDHKeyExchange
    {
        public TestTlsEcDhKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, int[] namedCurves, byte[] clientECPointFormats, byte[] serverECPointFormats) 
            : base(keyExchange, supportedSignatureAlgorithms, namedCurves, clientECPointFormats, serverECPointFormats)
        {
        }

        public ECPublicKeyParameters EcPublicKeyParameters => mECAgreePublicKey;
    }
}