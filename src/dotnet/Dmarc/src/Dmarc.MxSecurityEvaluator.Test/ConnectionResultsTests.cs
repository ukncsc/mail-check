using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Dao;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Processors;
using Dmarc.MxSecurityEvaluator.Util;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.MxSecurityEvaluator.Test
{
    [TestFixture]
    public class ConnectionResultsTests
    {
        [Test]
        public void WhenAllTestAreTcpConnectionFailedShouldReturnOneError()
        {
            var results = new ConnectionResults(new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null),
                new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null),
                new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null),
                new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null),
                new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null),
                new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null),
                new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null),
                new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null),
                new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null),
                new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null),
                new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null),
                new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, "", null));

            MxRecordTlsProfile profile = new MxRecordTlsProfile(1, "abc.com", DateTime.UtcNow, results);

            string failedConnectionErrors = profile.ConnectionResults.GetFailedConnectionErrors();
            Assert.IsTrue(string.IsNullOrWhiteSpace(failedConnectionErrors));
        }
    }
}
