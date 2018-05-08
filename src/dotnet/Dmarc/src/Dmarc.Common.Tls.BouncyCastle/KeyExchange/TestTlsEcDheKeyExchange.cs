using System.Collections;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Utilities.IO;

namespace Dmarc.Common.Tls.BouncyCastle.KeyExchange
{
    internal class TestTlsEcDheKeyExchange : TlsECDheKeyExchange
    {
        private SignatureAndHashAlgorithm mSignatureAndHashAlgorithm;

        public TestTlsEcDheKeyExchange(int keyExchange,
            IList supportedSignatureAlgorithms,
            int[] namedCurves,
            byte[] clientEcPointFormats,
            byte[] serverEcPointFormats)
            : base(keyExchange, supportedSignatureAlgorithms, namedCurves, clientEcPointFormats, serverEcPointFormats)
        {
        }

        public ECPublicKeyParameters EcPublicKeyParameters => mECAgreePublicKey;

        public SignatureAndHashAlgorithm EcSignatureAndHashAlgorithm => mSignatureAndHashAlgorithm;

        public override void ProcessServerKeyExchange(Stream input)
        {
            SecurityParameters securityParameters = mContext.SecurityParameters;

            SignerInputBuffer buf = new SignerInputBuffer();
            Stream teeIn = new TeeInputStream(input, buf);

            ECDomainParameters curveParams = TlsEccUtilities.ReadECParameters(mNamedCurves, mClientECPointFormats, teeIn);

            byte[] point = TlsUtilities.ReadOpaque8(teeIn);

            DigitallySigned signedParams = ParseSignature(input);

            mSignatureAndHashAlgorithm = signedParams.Algorithm;

            ISigner signer = InitVerifyer(mTlsSigner, signedParams.Algorithm, securityParameters);
            buf.UpdateSigner(signer);
            if (!signer.VerifySignature(signedParams.Signature))
                throw new TlsFatalAlert(AlertDescription.decrypt_error);

            mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey(TlsEccUtilities.DeserializeECPublicKey(
                mClientECPointFormats, curveParams, point));
        }
    }
}