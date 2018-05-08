using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities.IO;

namespace Dmarc.Common.Tls.BouncyCastle
{
    internal class SignerInputBuffer : MemoryStream
    {
        internal void UpdateSigner(ISigner s)
        {
            WriteTo(new SigStream(s));
        }

        private class SigStream : BaseOutputStream
        {
            private readonly ISigner _s;

            internal SigStream(ISigner s)
            {
                _s = s;
            }

            public override void WriteByte(byte b)
            {
                _s.Update(b);
            }

            public override void Write(byte[] buf, int off, int len)
            {
                _s.BlockUpdate(buf, off, len);
            }
        }
    }
}