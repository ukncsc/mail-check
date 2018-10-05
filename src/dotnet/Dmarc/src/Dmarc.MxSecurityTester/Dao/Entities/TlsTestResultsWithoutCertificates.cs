namespace Dmarc.MxSecurityTester.Dao.Entities
{
    public class TlsTestResultsWithoutCertificate
    {
        public TlsTestResultsWithoutCertificate(
            TlsTestResult tls12AvailableWithBestCipherSuiteSelected,
            TlsTestResult tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
            TlsTestResult tls12AvailableWithSha2HashFunctionSelected,
            TlsTestResult tls12AvailableWithWeakCipherSuiteNotSelected,
            TlsTestResult tls11AvailableWithBestCipherSuiteSelected,
            TlsTestResult tls11AvailableWithWeakCipherSuiteNotSelected,
            TlsTestResult tls10AvailableWithBestCipherSuiteSelected,
            TlsTestResult tls10AvailableWithWeakCipherSuiteNotSelected,
            TlsTestResult ssl3FailsWithBadCipherSuite,
            TlsTestResult tlsSecureEllipticCurveSelected,
            TlsTestResult tlsSecureDiffieHellmanGroupSelected,
            TlsTestResult tlsWeakCipherSuitesRejected)
        {
            Tls12AvailableWithBestCipherSuiteSelected = tls12AvailableWithBestCipherSuiteSelected;
            Tls12AvailableWithBestCipherSuiteSelectedFromReverseList =
                tls12AvailableWithBestCipherSuiteSelectedFromReverseList;
            Tls12AvailableWithSha2HashFunctionSelected = tls12AvailableWithSha2HashFunctionSelected;
            Tls12AvailableWithWeakCipherSuiteNotSelected = tls12AvailableWithWeakCipherSuiteNotSelected;
            Tls11AvailableWithBestCipherSuiteSelected = tls11AvailableWithBestCipherSuiteSelected;
            Tls11AvailableWithWeakCipherSuiteNotSelected = tls11AvailableWithWeakCipherSuiteNotSelected;
            Tls10AvailableWithBestCipherSuiteSelected = tls10AvailableWithBestCipherSuiteSelected;
            Tls10AvailableWithWeakCipherSuiteNotSelected = tls10AvailableWithWeakCipherSuiteNotSelected;
            Ssl3FailsWithBadCipherSuite = ssl3FailsWithBadCipherSuite;
            TlsSecureEllipticCurveSelected = tlsSecureEllipticCurveSelected;
            TlsSecureDiffieHellmanGroupSelected = tlsSecureDiffieHellmanGroupSelected;
            TlsWeakCipherSuitesRejected = tlsWeakCipherSuitesRejected;
        }

        public TlsTestResult Tls12AvailableWithBestCipherSuiteSelected { get; }
        public TlsTestResult Tls12AvailableWithBestCipherSuiteSelectedFromReverseList { get; }
        public TlsTestResult Tls12AvailableWithSha2HashFunctionSelected { get; }
        public TlsTestResult Tls12AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsTestResult Tls11AvailableWithBestCipherSuiteSelected { get; }
        public TlsTestResult Tls11AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsTestResult Tls10AvailableWithBestCipherSuiteSelected { get; }
        public TlsTestResult Tls10AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsTestResult Ssl3FailsWithBadCipherSuite { get; }
        public TlsTestResult TlsSecureEllipticCurveSelected { get; }
        public TlsTestResult TlsSecureDiffieHellmanGroupSelected { get; }
        public TlsTestResult TlsWeakCipherSuitesRejected { get; }

        protected bool Equals(TlsTestResultsWithoutCertificate other)
        {
            return Equals(Tls12AvailableWithBestCipherSuiteSelected,
                       other.Tls12AvailableWithBestCipherSuiteSelected) &&
                   Equals(Tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
                       other.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList) &&
                   Equals(Tls12AvailableWithSha2HashFunctionSelected,
                       other.Tls12AvailableWithSha2HashFunctionSelected) &&
                   Equals(Tls12AvailableWithWeakCipherSuiteNotSelected,
                       other.Tls12AvailableWithWeakCipherSuiteNotSelected) &&
                   Equals(Tls11AvailableWithBestCipherSuiteSelected, other.Tls11AvailableWithBestCipherSuiteSelected) &&
                   Equals(Tls11AvailableWithWeakCipherSuiteNotSelected,
                       other.Tls11AvailableWithWeakCipherSuiteNotSelected) &&
                   Equals(Tls10AvailableWithBestCipherSuiteSelected, other.Tls10AvailableWithBestCipherSuiteSelected) &&
                   Equals(Tls10AvailableWithWeakCipherSuiteNotSelected,
                       other.Tls10AvailableWithWeakCipherSuiteNotSelected) &&
                   Equals(Ssl3FailsWithBadCipherSuite, other.Ssl3FailsWithBadCipherSuite) &&
                   Equals(TlsSecureEllipticCurveSelected, other.TlsSecureEllipticCurveSelected) &&
                   Equals(TlsSecureDiffieHellmanGroupSelected, other.TlsSecureDiffieHellmanGroupSelected) &&
                   Equals(TlsWeakCipherSuitesRejected, other.TlsWeakCipherSuitesRejected);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TlsTestResultsWithoutCertificate)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Tls12AvailableWithBestCipherSuiteSelected != null
                    ? Tls12AvailableWithBestCipherSuiteSelected.GetHashCode()
                    : 0);
                hashCode = (hashCode * 397) ^ (Tls12AvailableWithBestCipherSuiteSelectedFromReverseList != null
                               ? Tls12AvailableWithBestCipherSuiteSelectedFromReverseList.GetHashCode()
                               : 0);
                hashCode = (hashCode * 397) ^ (Tls12AvailableWithSha2HashFunctionSelected != null
                               ? Tls12AvailableWithSha2HashFunctionSelected.GetHashCode()
                               : 0);
                hashCode = (hashCode * 397) ^ (Tls12AvailableWithWeakCipherSuiteNotSelected != null
                               ? Tls12AvailableWithWeakCipherSuiteNotSelected.GetHashCode()
                               : 0);
                hashCode = (hashCode * 397) ^ (Tls11AvailableWithBestCipherSuiteSelected != null
                               ? Tls11AvailableWithBestCipherSuiteSelected.GetHashCode()
                               : 0);
                hashCode = (hashCode * 397) ^ (Tls11AvailableWithWeakCipherSuiteNotSelected != null
                               ? Tls11AvailableWithWeakCipherSuiteNotSelected.GetHashCode()
                               : 0);
                hashCode = (hashCode * 397) ^ (Tls10AvailableWithBestCipherSuiteSelected != null
                               ? Tls10AvailableWithBestCipherSuiteSelected.GetHashCode()
                               : 0);
                hashCode = (hashCode * 397) ^ (Tls10AvailableWithWeakCipherSuiteNotSelected != null
                               ? Tls10AvailableWithWeakCipherSuiteNotSelected.GetHashCode()
                               : 0);
                hashCode = (hashCode * 397) ^
                           (Ssl3FailsWithBadCipherSuite != null ? Ssl3FailsWithBadCipherSuite.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TlsSecureEllipticCurveSelected != null
                               ? TlsSecureEllipticCurveSelected.GetHashCode()
                               : 0);
                hashCode = (hashCode * 397) ^ (TlsSecureDiffieHellmanGroupSelected != null
                               ? TlsSecureDiffieHellmanGroupSelected.GetHashCode()
                               : 0);
                hashCode = (hashCode * 397) ^
                           (TlsWeakCipherSuitesRejected != null ? TlsWeakCipherSuitesRejected.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}