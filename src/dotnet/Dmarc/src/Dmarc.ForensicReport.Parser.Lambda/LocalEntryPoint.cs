using System;
using System.Threading;
using Dmarc.Common.Report.Evnts;
using Dmarc.Common.Report.File;
using Dmarc.ForensicReport.Parser.Lambda.Console;
using Dmarc.ForensicReport.Parser.Lambda.Factory;
using Microsoft.Extensions.CommandLineUtils;

namespace Dmarc.ForensicReport.Parser.Lambda
{
    public class LocalEntryPoint
    {
        public static void Main(string[] args)
        {
            CommandLineApplication commandLineApplication = new CommandLineApplication(false);
            commandLineApplication.Name = "Forensic report processor";

            CommandOption directory = commandLineApplication.Option("-d |--directory <directory>", "The directory containing aggregate report emails", CommandOptionType.SingleValue);

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                CommandLineArgs commandLineArgs = new CommandLineArgs(directory.Value());

                IFileEmailMessageProcessor fileEmailMessageProcessor = ForensicReportParserAppFactory.Create();

                fileEmailMessageProcessor.ProcessEmailMessages(commandLineArgs.Directory);

                (fileEmailMessageProcessor as IDisposable)?.Dispose();

                return 0;
            });

            commandLineApplication.Command("Lambda", command =>
            {
                command.Description = "Run forensic report processor lambda code locally.";

                command.OnExecute(() =>
                {
                    ForensicReportProcessor forensicReportProcessor = new ForensicReportProcessor();

                    while (true)
                    {
                        forensicReportProcessor.HandleScheduledEvent(ScheduledEvent.EmptyScheduledEvent,
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
