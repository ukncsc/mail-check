using System.Collections.Generic;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using NUnit.Framework;

namespace Dmarc.Common.Test.Logging
{
    [TestFixture]
    public class AbstractLoggerTests
    {
        private const string Trace = "TraceMessage";
        private const string Debug = "DebugMessage";
        private const string Info = "InfoMessage";
        private const string Warn = "WarnMessage";
        private const string Error = "ErrorMessage";

        private ConcreateLogger _concreateLogger;

        [SetUp]
        public void SetUp()
        {
            _concreateLogger = new ConcreateLogger();
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public void Test(LogLevel level, List<string> expected)
        {
            ConcreateLogger.Messages.Clear();
            _concreateLogger.Level = level;
            _concreateLogger.Trace(Trace);
            _concreateLogger.Debug(Debug);
            _concreateLogger.Info(Info);
            _concreateLogger.Warn(Warn);
            _concreateLogger.Error(Error);

            Assert.That(ConcreateLogger.Messages.Count, Is.EqualTo(expected.Count));
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.That(ConcreateLogger.Messages[i].EndsWith(expected[i]));
            }
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            yield return new TestCaseData(LogLevel.Trace, new List<string> {Trace, Debug, Info, Warn, Error }).SetName("log level trace logs trace - error");
            yield return new TestCaseData(LogLevel.Debug, new List<string> { Debug, Info, Warn, Error }).SetName("log level debug logs debug - error");
            yield return new TestCaseData(LogLevel.Info, new List<string> { Info, Warn, Error }).SetName("log level info logs info - error");
            yield return new TestCaseData(LogLevel.Warn, new List<string> { Warn, Error }).SetName("log level warn logs warn - error");
            yield return new TestCaseData(LogLevel.Error, new List<string> { Error }).SetName("log level error logs error");
        }

        private class ConcreateLogger : AbstractLogger
        {
            public static List<string> Messages { get; } = new List<string>();

            public ConcreateLogger() : base(s => Messages.Add(s)){}
        }
    }
}
