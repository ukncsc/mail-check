using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityEvaluator.Domain
{
    public class ConnectionResults
    {
        public ConnectionResults(TlsConnectionResult tls12AvailableWithBestCipherSuiteSelected,
            TlsConnectionResult tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
            TlsConnectionResult tls12AvailableWithSha2HashFunctionSelected,
            TlsConnectionResult tls12AvailableWithWeakCipherSuiteNotSelected,
            TlsConnectionResult tls11AvailableWithBestCipherSuiteSelected,
            TlsConnectionResult tls11AvailableWithWeakCipherSuiteNotSelected,
            TlsConnectionResult tls10AvailableWithBestCipherSuiteSelected,
            TlsConnectionResult tls10AvailableWithWeakCipherSuiteNotSelected,
            TlsConnectionResult ssl3FailsWithBadCipherSuite,
            TlsConnectionResult tlsSecureEllipticCurveSelected,
            TlsConnectionResult tlsSecureDiffieHellmanGroupSelected,
            TlsConnectionResult tlsWeakCipherSuitesRejected)
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

        public TlsConnectionResult Tls12AvailableWithBestCipherSuiteSelected { get; }
        public TlsConnectionResult Tls12AvailableWithBestCipherSuiteSelectedFromReverseList { get; }
        public TlsConnectionResult Tls12AvailableWithSha2HashFunctionSelected { get; }
        public TlsConnectionResult Tls12AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsConnectionResult Tls11AvailableWithBestCipherSuiteSelected { get; }
        public TlsConnectionResult Tls11AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsConnectionResult Tls10AvailableWithBestCipherSuiteSelected { get; }
        public TlsConnectionResult Tls10AvailableWithWeakCipherSuiteNotSelected { get; }
        public TlsConnectionResult Ssl3FailsWithBadCipherSuite { get; }
        public TlsConnectionResult TlsSecureEllipticCurveSelected { get; }
        public TlsConnectionResult TlsSecureDiffieHellmanGroupSelected { get; }
        public TlsConnectionResult TlsWeakCipherSuitesRejected { get; }
        
        public bool HasFailedConnection()
        {
            return TlsHasFailedConnection(Tls12AvailableWithBestCipherSuiteSelected)
                   && TlsHasFailedConnection(Tls12AvailableWithBestCipherSuiteSelectedFromReverseList)
                   && TlsHasFailedConnection(Tls12AvailableWithSha2HashFunctionSelected)
                   && TlsHasFailedConnection(Tls12AvailableWithWeakCipherSuiteNotSelected)
                   && TlsHasFailedConnection(Tls11AvailableWithBestCipherSuiteSelected)
                   && TlsHasFailedConnection(Tls11AvailableWithWeakCipherSuiteNotSelected)
                   && TlsHasFailedConnection(Tls10AvailableWithBestCipherSuiteSelected)
                   && TlsHasFailedConnection(Tls10AvailableWithWeakCipherSuiteNotSelected)
                   && TlsHasFailedConnection(Ssl3FailsWithBadCipherSuite)
                   && TlsHasFailedConnection(TlsSecureEllipticCurveSelected)
                   && TlsHasFailedConnection(TlsSecureDiffieHellmanGroupSelected)
                   && TlsHasFailedConnection(TlsWeakCipherSuitesRejected);
        }

        private bool TlsHasFailedConnection(TlsConnectionResult result)
        {
            return result.Error == Error.SESSION_INITIALIZATION_FAILED || result.Error == Error.TCP_CONNECTION_FAILED;
        }
    }
}
