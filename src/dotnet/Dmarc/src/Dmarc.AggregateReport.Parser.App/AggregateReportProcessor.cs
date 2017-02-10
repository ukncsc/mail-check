using System;
using System.Diagnostics;
using Dmarc.AggregateReport.Parser.App.Factory;
using Dmarc.AggregateReport.Parser.App.Logging;
using Dmarc.AggregateReport.Parser.App.Parsers;
using Dmarc.Common.Logging;
using Microsoft.Extensions.CommandLineUtils;

namespace Dmarc.AggregateReport.Parser.App
{
    public class AggregateReportProcessor
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
            commandLineApplication.Execute(args);
        }
    }
}
