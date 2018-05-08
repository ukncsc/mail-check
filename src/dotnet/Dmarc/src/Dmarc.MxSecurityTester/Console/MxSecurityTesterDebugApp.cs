using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.MxSecurityTester.MxTester;

namespace Dmarc.MxSecurityTester.Console
{
    internal interface IMxSecurityTesterDebugApp
    {
        Task Run(List<string> hosts);
    }

    internal class MxSecurityTesterDebugApp : IMxSecurityTesterDebugApp
    {
        private readonly ITlsSecurityTester _tlsSecurityTester;
        private readonly ILogger _log;

        public MxSecurityTesterDebugApp(ITlsSecurityTester tlsSecurityTester,
            ILogger log)
        {
            _tlsSecurityTester = tlsSecurityTester;
            _log = log;
        }

        public async Task Run(List<string> hosts)
        {
            foreach (string host in hosts)
            {
                _log.Debug($"Testing TLS for {host}");
                List<TlsTestResult> testlResults = await _tlsSecurityTester.Test(host);
                foreach (var testlResult in testlResults)
                {
                    _log.Debug($"{testlResult.Test.Id} : {testlResult.Test.Name},");
                    _log.Debug($"\t{testlResult.Result}");
                }
            }
        }
    }
}
