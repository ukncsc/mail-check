using Dmarc.MxSecurityTester.Factory;
using Dmarc.MxSecurityTester.MxTester;
using NUnit.Framework;

namespace Dmarc.MxSecurityTester.Test.Factory
{
    [TestFixture]
    public class MxSecurityTesterFactoryTests
    {
        [Test]
        public void CanCreateTlsCompatibilityProcessor()
        {
            System.Environment.SetEnvironmentVariable("MxRecordLimit", "1");
            System.Environment.SetEnvironmentVariable("RefreshIntervalSeconds", "1");
            System.Environment.SetEnvironmentVariable("FailureRefreshIntervalSeconds", "1");
            System.Environment.SetEnvironmentVariable("TlsTestTimeoutSeconds", "1");
            System.Environment.SetEnvironmentVariable("SchedulerRunIntervalSeconds", "1");
            System.Environment.SetEnvironmentVariable("SmtpHostName", "localhost");
            System.Environment.SetEnvironmentVariable("CacheHostName", "localhost");
            System.Environment.SetEnvironmentVariable("SnsTopicArn", "localhost");
            System.Environment.SetEnvironmentVariable("SnsCertsTopicArn", "localhost");

            IMxSecurityTesterProcessorRunner mxSecurityTesterProcessorRunner = MxSecurityTesterFactory.CreateMxSecurityTesterProcessorRunner();
            Assert.That(mxSecurityTesterProcessorRunner, Is.Not.Null);
        }
    }
}
