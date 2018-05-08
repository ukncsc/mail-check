using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Tls;

namespace Dmarc.MxSecurityTester.Console
{
    public class TlsTestResult
    {
        public TlsTestResult(ITlsTest test, TlsConnectionResult result)
        {
            Test = test;
            Result = result;
        }

        public ITlsTest Test { get; }
        public TlsConnectionResult Result { get; }
    }
}