using System;

namespace Dmarc.MxSecurityTester.Util
{
    public enum TlsTestType
    {
        Tls12AvailableWithBestCipherSuiteSelected = 1,
        Tls12AvailableWithBestCipherSuiteSelectedFromReverseList = 2,
        Tls12AvailableWithSha2HashFunctionSelected = 3,
        Tls12AvailableWithWeakCipherSuiteNotSelected = 4,
        Tls11AvailableWithBestCipherSuiteSelected = 5,
        Tls11AvailableWithWeakCipherSuiteNotSelected = 6,
        Tls10AvailableWithBestCipherSuiteSelected = 7,
        Tls10AvailableWithWeakCipherSuiteNotSelected = 8,
        Ssl3FailsWithBadCipherSuite = 9,
        TlsSecureEllipticCurveSelected = 10,
        TlsSecureDiffieHellmanGroupSelected = 11,
        TlsWeakCipherSuitesRejected = 12
    }
}
