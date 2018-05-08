using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Console;
using Dmarc.MxSecurityTester.Tls;

namespace Dmarc.MxSecurityTester.MxTester
{
    internal interface ITlsSecurityTester
    {
        Task<List<TlsTestResult>> Test(string host);
    }

    internal class TlsSecurityTester : ITlsSecurityTester
    {
        private const int Port = 25;

        private readonly List<ITlsTest> _tests;
        private readonly ITlsClient _client;
        private readonly ILogger _log;

        public TlsSecurityTester(IEnumerable<ITlsTest> tests,
            ITlsClient client,
            ILogger log)
        {
            _tests = tests.OrderBy(_ => _.Id).ToList();
            _client = client;
            _log = log;
        }

        public async Task<List<TlsTestResult>> Test(string host)
        {
            List<TlsTestResult> testResults = new List<TlsTestResult>();

            foreach (ITlsTest test in _tests)
            {
                _log.Debug($"Running test {test.Id} - {test.Name} for {host ?? "null"}");

                TlsConnectionResult result = await _client.Connect(host, Port, test.Version, test.CipherSuites);
                _client.Disconnect();

                _log.Debug($"Result:{Environment.NewLine}{result}");

                testResults.Add(new TlsTestResult(test, result));
            }

            return testResults;
        }
    }
}