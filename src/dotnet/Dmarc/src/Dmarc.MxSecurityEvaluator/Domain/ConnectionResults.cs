using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityEvaluator.Domain
{
    public class ConnectionResults
    {
        private readonly List<TlsConnectionResult> _results;

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
            _results = new List<TlsConnectionResult>
            {
                tls12AvailableWithBestCipherSuiteSelected,
                tls12AvailableWithBestCipherSuiteSelectedFromReverseList,
                tls12AvailableWithSha2HashFunctionSelected,
                tls12AvailableWithWeakCipherSuiteNotSelected,
                tls11AvailableWithBestCipherSuiteSelected,
                tls11AvailableWithWeakCipherSuiteNotSelected,
                tls10AvailableWithBestCipherSuiteSelected,
                tls10AvailableWithWeakCipherSuiteNotSelected,
                ssl3FailsWithBadCipherSuite,
                tlsSecureEllipticCurveSelected,
                tlsSecureDiffieHellmanGroupSelected,
                tlsWeakCipherSuitesRejected
            };
        }

        public TlsConnectionResult Tls12AvailableWithBestCipherSuiteSelected => _results[0];
        public TlsConnectionResult Tls12AvailableWithBestCipherSuiteSelectedFromReverseList => _results[1];
        public TlsConnectionResult Tls12AvailableWithSha2HashFunctionSelected => _results[2];
        public TlsConnectionResult Tls12AvailableWithWeakCipherSuiteNotSelected => _results[3];
        public TlsConnectionResult Tls11AvailableWithBestCipherSuiteSelected => _results[4];
        public TlsConnectionResult Tls11AvailableWithWeakCipherSuiteNotSelected => _results[5];
        public TlsConnectionResult Tls10AvailableWithBestCipherSuiteSelected => _results[6];
        public TlsConnectionResult Tls10AvailableWithWeakCipherSuiteNotSelected => _results[7];
        public TlsConnectionResult Ssl3FailsWithBadCipherSuite => _results[8];
        public TlsConnectionResult TlsSecureEllipticCurveSelected => _results[9];
        public TlsConnectionResult TlsSecureDiffieHellmanGroupSelected => _results[10];
        public TlsConnectionResult TlsWeakCipherSuitesRejected => _results[11];

        public bool HostNotFound() =>
            _results.All(_ => _.Error == Error.HOST_NOT_FOUND);

        public bool HasFailedConnection() =>
            _results.All(_ => _.Error == Error.SESSION_INITIALIZATION_FAILED ||
                _.Error == Error.TCP_CONNECTION_FAILED);

        public string GetFailedConnectionErrors() =>
            string.Join(", ", _results
                .Select(_ => _.ErrorDescription)
                .Distinct()
                .ToList());
    }
}
