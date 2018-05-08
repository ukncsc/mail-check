using System.Collections.Generic;
using Dmarc.AggregateReport.Parser.Lambda;
using Dmarc.AggregateReport.Parser.Lambda.Factory;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.File;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Factory
{
    [TestFixture]
    internal class AggregateReportParserAppFactoryTests
    {
        [TestCaseSource(nameof(CreateTestData))]
        public void TestCreation(CommandLineArgs commandLineArgs)
        {
            IFileEmailMessageProcessor fileEmailMessageProcessor = AggregateReportParserAppFactory.Create(commandLineArgs, A.Fake<ILogger>());
            Assert.That(fileEmailMessageProcessor, Is.Not.Null);
        }

        public static IEnumerable<TestCaseData> CreateTestData()
        {
            yield return new TestCaseData(new CommandLineArgs(null, null, null, null)).SetName("create file aggregate report parser with null commandline args");
            yield return new TestCaseData(new CommandLineArgs("C:\\", null, null, null)).SetName("create file aggregate report parser with directory commandline args");
            yield return new TestCaseData(new CommandLineArgs(null, "C:\\", null, null)).SetName("create file aggregate report parser with xml directory commandline args");
            yield return new TestCaseData(new CommandLineArgs(null, null, "C:\\", null)).SetName("create file aggregate report parser with csv file commandline args");
            yield return new TestCaseData(new CommandLineArgs(null, null, null, "C:\\")).SetName("create file aggregate report parser with sql file commandline args");
        }
    }
}
