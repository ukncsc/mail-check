using System.Collections.Generic;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Util;

namespace Dmarc.MxSecurityTester.Tls.Tests
{
    public class TlsSecureDiffieHelmanGroupSelected : ITlsTest
    {
        public int Id => (int)TlsTestType.TlsSecureDiffieHellmanGroupSelected;

        public string Name => nameof(TlsSecureDiffieHelmanGroupSelected);

        public TlsVersion Version => TlsVersion.TlsV12;

        public List<CipherSuite> CipherSuites => new List<CipherSuite>
        {
            CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384,
            CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256,
            CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA,
            CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA,
        };
    }
}