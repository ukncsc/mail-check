using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Dao.Entities;
using Dmarc.MxSecurityTester.Mappers;
using Dmarc.MxSecurityTester.Util;
using Certificate = Dmarc.MxSecurityTester.Dao.Entities.Certificate;
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
            List<Console.TlsTestResult> results = await _tlsSecurityTester.Test(mxRecordTlsSecurityProfile.MxRecord.Hostname);

            List<Certificate> certificates =
               results.FirstOrDefault(_ => _.Result.Certificates.Any())?
                   .Result.Certificates.Select(_ => _.MapCertificate(true))
                   .ToList() ?? new List<Certificate>();

            return new MxRecordTlsSecurityProfile(mxRecordTlsSecurityProfile.MxRecord,
                    new TlsSecurityProfile(
                        mxRecordTlsSecurityProfile.TlsSecurityProfile.Id, 
                        null,
                        new TlsTestResults(
                            IsErrored(results)
                            ? mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.FailureCount + 1
                            : 0,
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.Tls12AvailableWithBestCipherSuiteSelected)),
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList)),
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.Tls12AvailableWithSha2HashFunctionSelected)),
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.Tls12AvailableWithWeakCipherSuiteNotSelected)),
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.Tls11AvailableWithBestCipherSuiteSelected)),
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.Tls11AvailableWithWeakCipherSuiteNotSelected)),
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.Tls10AvailableWithBestCipherSuiteSelected)),
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.Tls10AvailableWithWeakCipherSuiteNotSelected)),
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.Ssl3FailsWithBadCipherSuite)),
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.TlsSecureEllipticCurveSelected)),
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.TlsSecureDiffieHellmanGroupSelected)),
                        ToTestResult(results.FirstOrDefault(_ => _.Test.Id == (int)TlsTestType.TlsWeakCipherSuitesRejected)),
                        certificates)));
        }

        private TlsTestResult ToTestResult(Console.TlsTestResult tlsTestResult)
        {
            return tlsTestResult == null 
                ? new TlsTestResult(null, null, null, null, null)
                : new TlsTestResult(tlsTestResult.Result.Version,
                tlsTestResult.Result.CipherSuite,
                tlsTestResult.Result.CurveGroup,
                tlsTestResult.Result.SignatureHashAlgorithm,
                tlsTestResult.Result.Error);
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