using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class TlsResults
    {
        public TlsResults(TlsEvaluatorResult tls12AvailableWithBestCipherSuiteSelected,
            TlsEvaluatorResult tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
            TlsEvaluatorResult tls12AvailableWithSha2HashFunctionSelected,
            TlsEvaluatorResult tls12AvailableWithWeakCipherSuiteNotSelected,
            TlsEvaluatorResult tls11AvailableWithBestCipherSuiteSelected,
            TlsEvaluatorResult tls11AvailableWithWeakCipherSuiteNotSelected,
            TlsEvaluatorResult tls10AvailableWithBestCipherSuiteSelected,
            TlsEvaluatorResult tls10AvailableWithWeakCipherSuiteNotSelected,
            TlsEvaluatorResult ssl3FailsWithBadCipherSuite,
            TlsEvaluatorResult tlsSecureEllipticCurveSelected,
            TlsEvaluatorResult tlsSecureDiffieHellmanGroupSelected,
            TlsEvaluatorResult tlsWeakCipherSuitesRejected,
            int tlsStatus)
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
            TlsStatus = tlsStatus;
        }

        public TlsEvaluatorResult Tls12AvailableWithBestCipherSuiteSelected { get; }
        public TlsEvaluatorResult Tls12AvailableWithBestCipherSuiteSelectedFromReverseList { get; }
        public TlsEvaluatorResult Tls12AvailableWithSha2HashFunctionSelected { get; }
        public TlsEvaluatorResult Tls12AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsEvaluatorResult Tls11AvailableWithBestCipherSuiteSelected { get; }
        public TlsEvaluatorResult Tls11AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsEvaluatorResult Tls10AvailableWithBestCipherSuiteSelected { get; }
        public TlsEvaluatorResult Tls10AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsEvaluatorResult Ssl3FailsWithBadCipherSuite { get; }
        public TlsEvaluatorResult TlsSecureEllipticCurveSelected { get; }
        public TlsEvaluatorResult TlsSecureDiffieHellmanGroupSelected { get; }
        public TlsEvaluatorResult TlsWeakCipherSuitesRejected { get; }
        public int TlsStatus { get; }

        protected bool Equals(TlsResults other)
        {
            return Equals(Tls12AvailableWithBestCipherSuiteSelected, other.Tls12AvailableWithBestCipherSuiteSelected) &&
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
            return Equals((TlsResults) obj);
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

        public static bool operator ==(TlsResults left, TlsResults right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TlsResults left, TlsResults right)
        {
            return !Equals(left, right);
        }
    }
}