using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Dao.Entities;
using Dmarc.MxSecurityTester.Util;
using TlsTestResult = Dmarc.MxSecurityTester.Dao.Entities.TlsTestResult;

namespace Dmarc.MxSecurityTester.MxTester
{
    internal interface ITlsSecurityTesterAdapator
    {
        Task<MxRecordTlsSecurityProfile> Test(MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile);
    }

    internal class TlsSecurityTesterAdapator : ITlsSecurityTesterAdapator
    {
        private readonly ITlsSecurityTester _tlsSecurityTester;

        public TlsSecurityTesterAdapator(ITlsSecurityTester tlsSecurityTester)
        {
            _tlsSecurityTester = tlsSecurityTester;
        }

        public async Task<MxRecordTlsSecurityProfile> Test(MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile)
        {
            List<Console.TlsTestResult> results = new List<Console.TlsTestResult>();
            List<X509Certificate2> certificates = null;

            if (!string.IsNullOrWhiteSpace(mxRecordTlsSecurityProfile.MxRecord.Hostname))
            {
                results = await _tlsSecurityTester.Test(mxRecordTlsSecurityProfile.MxRecord.Hostname);

                certificates = results.FirstOrDefault(_ => _.Result.Certificates.Any())?
                                   .Result.Certificates.ToList() ?? new List<X509Certificate2>();
            }

            return new MxRecordTlsSecurityProfile(mxRecordTlsSecurityProfile.MxRecord,
                new TlsSecurityProfile(
                    mxRecordTlsSecurityProfile.TlsSecurityProfile.Id,
                    null,
                    new TlsTestResults(
                        IsErrored(results)
                            ? mxRecordTlsSecurityProfile.TlsSecurityProfile.TlsResults.FailureCount + 1
                            : 0, new TlsTestResultsWithoutCertificate(
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id == (int) TlsTestType.Tls12AvailableWithBestCipherSuiteSelected)),
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id ==
                                (int) TlsTestType.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList)),
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id == (int) TlsTestType.Tls12AvailableWithSha2HashFunctionSelected)),
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id == (int) TlsTestType.Tls12AvailableWithWeakCipherSuiteNotSelected)),
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id == (int) TlsTestType.Tls11AvailableWithBestCipherSuiteSelected)),
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id == (int) TlsTestType.Tls11AvailableWithWeakCipherSuiteNotSelected)),
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id == (int) TlsTestType.Tls10AvailableWithBestCipherSuiteSelected)),
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id == (int) TlsTestType.Tls10AvailableWithWeakCipherSuiteNotSelected)),
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id == (int) TlsTestType.Ssl3FailsWithBadCipherSuite)),
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id == (int) TlsTestType.TlsSecureEllipticCurveSelected)),
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id == (int) TlsTestType.TlsSecureDiffieHellmanGroupSelected)),
                            ToTestResult(results.FirstOrDefault(_ =>
                                _.Test.Id == (int) TlsTestType.TlsWeakCipherSuitesRejected))),
                        certificates)));
        }

        private TlsTestResult ToTestResult(Console.TlsTestResult tlsTestResult)
        {
            return tlsTestResult == null 
                ? new TlsTestResult(null, null, null, null, null,null, null)
                : new TlsTestResult(tlsTestResult.Result.Version,
                tlsTestResult.Result.CipherSuite,
                tlsTestResult.Result.CurveGroup,
                tlsTestResult.Result.SignatureHashAlgorithm,
                tlsTestResult.Result.Error,
                    tlsTestResult.Result.ErrorDescription, tlsTestResult.Result.SmtpResponses);
        }

        private bool IsErrored(List<Console.TlsTestResult> testResults)
        {
            return testResults.Any(IsErrored);
        }

        private bool IsErrored(Console.TlsTestResult testResult)
        {
            return testResult.Result.Error == Error.TCP_CONNECTION_FAILED ||
                   testResult.Result.Error == Error.SESSION_INITIALIZATION_FAILED;
        }
    }
}