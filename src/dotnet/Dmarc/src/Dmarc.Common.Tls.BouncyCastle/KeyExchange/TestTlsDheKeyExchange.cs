using System.Collections;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Utilities.IO;

namespace Dmarc.Common.Tls.BouncyCastle.KeyExchange
{
    internal class TestTlsDheKeyExchange : TlsDheKeyExchange
    {
        private SignatureAndHashAlgorithm mSignatureAndHashAlgorithm;

        public TestTlsDheKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, DHParameters dhParameters) 
            : base(keyExchange, supportedSignatureAlgorithms, dhParameters)
        {
        }

        public DHParameters DhParameters => mDHParameters;

        public SignatureAndHashAlgorithm EcSignatureAndHashAlgorithm => mSignatureAndHashAlgorithm;

        public override void ProcessServerKeyExchange(Stream input)
        {
            SecurityParameters securityParameters = mContext.SecurityParameters;

            SignerInputBuffer buf = new SignerInputBuffer();
            Stream teeIn = new TeeInputStream(input, buf);

            ServerDHParams dhParams = ServerDHParams.Parse(teeIn);

            DigitallySigned signed_params = ParseSignature(input);

            mSignatureAndHashAlgorithm = signed_params.Algorithm;

            ISigner signer = InitVerifyer(mTlsSigner, signed_params.Algorithm, securityParameters);
            buf.UpdateSigner(signer);
            if (!signer.VerifySignature(signed_params.Signature))
                throw new TlsFatalAlert(AlertDescription.decrypt_error);

            this.mDHAgreePublicKey = TlsDHUtilities.ValidateDHPublicKey(dhParams.PublicKey);
            this.mDHParameters = ValidateDHParameters(mDHAgreePublicKey.Parameters);
        }
    }
}