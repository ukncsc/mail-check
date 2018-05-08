using System;
using System.Diagnostics;
using System.Threading;
using Dmarc.AggregateReport.Parser.Lambda.Factory;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.Common.Report.Evnts;
using Dmarc.Common.Report.File;
using Microsoft.Extensions.CommandLineUtils;

namespace Dmarc.AggregateReport.Parser.Lambda
{
    public class LocalEntryPoint
    {
        public static void Main(string[] args)
        {
            CommandLineApplication commandLineApplication = new CommandLineApplication(false);

            CommandOption directory = commandLineApplication.Option("-d |--directory <directory>", "The directory containing aggregate report emails", CommandOptionType.SingleValue);
            CommandOption xmlDirectory = commandLineApplication.Option("-x |--xml <xmldirectory>", "The directory to write xml aggregate reports to", CommandOptionType.SingleValue);
            CommandOption csvFile = commandLineApplication.Option("-c |--csv <csvfile>", "The csv file to write denormalised records to", CommandOptionType.SingleValue);
            CommandOption sqlFile = commandLineApplication.Option("-s |--sqlite <sqlitefile>", "The sqlite file to write denormalised records to", CommandOptionType.SingleValue);

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                CommandLineArgs commandLineArgs = new CommandLineArgs(
                    directory.Value(),
                    xmlDirectory.Value(),
                    csvFile.Value(),
                    sqlFile.Value());

                ILogger log = new ConsoleLogger();

                Stopwatch stopwatch = Stopwatch.StartNew();
                IFileEmailMessageProcessor fileEmailMessageProcessor = AggregateReportParserAppFactory.Create(commandLineArgs, log);
                TimeSpan createAggregateReportParserTimeSpan = stopwatch.Elapsed;

                stopwatch.Restart();

                fileEmailMessageProcessor.ProcessEmailMessages(commandLineArgs.Directory);
                TimeSpan parseTimeSpan = stopwatch.Elapsed;
                stopwatch.Stop();

                log.Debug($"Creating parser took: {createAggregateReportParserTimeSpan}");
                log.Debug($"Parsing took: {parseTimeSpan}");

                (fileEmailMessageProcessor as IDisposable)?.Dispose();

                return 0;
            });

            commandLineApplication.Command("Lambda", command =>
            {
                command.Description = "Run aggregate report processor lambda code locally.";

                command.OnExecute(() =>
                {
                    AggregateReportProcessor aggregateReportProcessor = new AggregateReportProcessor();

                    while (true)
                    {
                        aggregateReportProcessor.HandleScheduledEvent(ScheduledEvent.EmptyScheduledEvent,
                            LambdaContext.NonExpiringLambda).Wait();

                        Thread.Sleep(10000);
                    }

                    return 0;
                });

            }, false);


            commandLineApplication.Execute(args);
        }
    }
}
