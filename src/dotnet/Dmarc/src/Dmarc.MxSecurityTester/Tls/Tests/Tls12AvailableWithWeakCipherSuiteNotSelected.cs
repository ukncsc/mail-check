using System.Collections.Generic;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Util;

namespace Dmarc.MxSecurityTester.Tls.Tests
{
    public class Tls12AvailableWithWeakCipherSuiteNotSelected : ITlsTest
    {
        public int Id => (int)TlsTestType.Tls12AvailableWithWeakCipherSuiteNotSelected;

        public string Name => nameof(Tls12AvailableWithWeakCipherSuiteNotSelected);

        public TlsVersion Version => TlsVersion.TlsV12;

        public List<CipherSuite> CipherSuites => new List<CipherSuite>
        {
            CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA,
            CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA,
            CipherSuite.TLS_RSA_WITH_RC4_128_SHA,
            CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA,
            CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA,
            CipherSuite.TLS_NULL_WITH_NULL_NULL,
            CipherSuite.TLS_RSA_WITH_NULL_MD5,
            CipherSuite.TLS_RSA_WITH_RC4_128_MD5,
            CipherSuite.TLS_RSA_WITH_NULL_SHA,
            CipherSuite.TLS_RSA_EXPORT_WITH_RC4_40_MD5,
            CipherSuite.TLS_RSA_EXPORT_WITH_RC2_CBC_40_MD5,
            CipherSuite.TLS_RSA_EXPORT_WITH_DES40_CBC_SHA,
            CipherSuite.TLS_RSA_WITH_DES_CBC_SHA,
            CipherSuite.TLS_DH_DSS_EXPORT_WITH_DES40_CBC_SHA,
            CipherSuite.TLS_DH_DSS_WITH_DES_CBC_SHA,
            CipherSuite.TLS_DH_RSA_EXPORT_WITH_DES40_CBC_SHA,
            CipherSuite.TLS_DH_RSA_WITH_DES_CBC_SHA,
            CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA,
            CipherSuite.TLS_DHE_DSS_WITH_DES_CBC_SHA,
            CipherSuite.TLS_DHE_RSA_EXPORT_WITH_DES40_CBC_SHA,
            CipherSuite.TLS_DHE_RSA_WITH_DES_CBC_SHA,
        };
    }
}