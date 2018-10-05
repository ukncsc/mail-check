using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.MxSecurityEvaluator.Factory;
using Dmarc.MxSecurityEvaluator.Processors;
using Microsoft.Extensions.CommandLineUtils;

namespace Dmarc.MxSecurityEvaluator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CommandLineApplication commandLineApplication = new CommandLineApplication(false);
            commandLineApplication.Name = "MxSecurityEvaluator";

            commandLineApplication.Command("debug", command =>
            {
                command.HelpOption("-? | -h | --help");

                command.Description = "Debug tls evaluator for mx records.";

                CommandArgument domainId = command.Argument("id", "the domain ID to evaluate", true);

                command.OnExecute(() =>
                {
                    List<int> ids = domainId.Values.Select(int.Parse).ToList();

                    ITlsRecordProcessor tslRecordProcessor = MxSecurityEvaluatorFactory.CreateManualProcessor(ids);

                    tslRecordProcessor.Run().Wait();

                    Console.WriteLine("Press any key...");

                    Console.ReadKey();

                    return 0;
                });
            });

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                ITlsRecordProcessor tlsRecordProcessor = MxSecurityEvaluatorFactory.CreateQueueProcessor();

                tlsRecordProcessor.Run().Wait();

                return 0;
            });

            commandLineApplication.Execute(args);
        }
    }
}
