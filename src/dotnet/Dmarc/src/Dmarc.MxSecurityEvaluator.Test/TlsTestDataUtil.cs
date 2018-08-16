using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Util;

namespace Dmarc.MxSecurityEvaluator.Test
{
    public class TlsTestDataUtil
    {
        private static TlsConnectionResult SetupConnectionResult(IDictionary<TlsTestType, TlsConnectionResult> data, TlsTestType testType)
        {
            return data.ContainsKey(testType) 
                ? data[testType]
                : new TlsConnectionResult(Error.BAD_CERTIFICATE, "Bad certificate found", null);
        }

        public static ConnectionResults CreateConnectionResults(IDictionary<TlsTestType, TlsConnectionResult> data)
        {
            return new ConnectionResults(
                SetupConnectionResult(data, TlsTestType.Tls12AvailableWithBestCipherSuiteSelected),
                SetupConnectionResult(data, TlsTestType.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList),
                SetupConnectionResult(data, TlsTestType.Tls12AvailableWithSha2HashFunctionSelected),
                SetupConnectionResult(data, TlsTestType.Tls12AvailableWithWeakCipherSuiteNotSelected),
                SetupConnectionResult(data, TlsTestType.Tls11AvailableWithBestCipherSuiteSelected),
                SetupConnectionResult(data, TlsTestType.Tls11AvailableWithWeakCipherSuiteNotSelected),
                SetupConnectionResult(data, TlsTestType.Tls10AvailableWithBestCipherSuiteSelected),
                SetupConnectionResult(data, TlsTestType.Tls10AvailableWithWeakCipherSuiteNotSelected),
                SetupConnectionResult(data, TlsTestType.Ssl3FailsWithBadCipherSuite),
                SetupConnectionResult(data, TlsTestType.TlsSecureEllipticCurveSelected),
                SetupConnectionResult(data, TlsTestType.TlsSecureDiffieHellmanGroupSelected),
                SetupConnectionResult(data, TlsTestType.TlsWeakCipherSuitesRejected));

        }

        public static ConnectionResults CreateConnectionResults(TlsTestType testType, TlsConnectionResult data)
        {
            return new ConnectionResults(
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() { { testType, data } }, TlsTestType.Tls12AvailableWithBestCipherSuiteSelected),
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() {{testType, data}}, TlsTestType.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList),
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() { { testType, data } }, TlsTestType.Tls12AvailableWithSha2HashFunctionSelected),
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() { { testType, data } }, TlsTestType.Tls12AvailableWithWeakCipherSuiteNotSelected),
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() { { testType, data } }, TlsTestType.Tls11AvailableWithBestCipherSuiteSelected),
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() { { testType, data } }, TlsTestType.Tls11AvailableWithWeakCipherSuiteNotSelected),
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() { { testType, data } }, TlsTestType.Tls10AvailableWithBestCipherSuiteSelected),
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() { { testType, data } }, TlsTestType.Tls10AvailableWithWeakCipherSuiteNotSelected),
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() { { testType, data } }, TlsTestType.Ssl3FailsWithBadCipherSuite),
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() { { testType, data } }, TlsTestType.TlsSecureEllipticCurveSelected),
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() { { testType, data } }, TlsTestType.TlsSecureDiffieHellmanGroupSelected),
                SetupConnectionResult(new Dictionary<TlsTestType, TlsConnectionResult>() { { testType, data } }, TlsTestType.TlsWeakCipherSuitesRejected));
        }
    }
}
