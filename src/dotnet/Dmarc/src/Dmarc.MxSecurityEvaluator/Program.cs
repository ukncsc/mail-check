using Dmarc.MxSecurityEvaluator.Factory;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Linq;

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
                    var ids = domainId.Values.Select(int.Parse).ToList();

                    var tslRecordProcessor = MxSecurityEvaluatorFactory.CreateManualProcessor(ids);

                    tslRecordProcessor.Run().Wait();

                    Console.WriteLine("Press any key...");

                    Console.ReadKey();

                    return 0;
                });
            },
            false);

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                var tlsRecordProcessor = MxSecurityEvaluatorFactory.CreateQueueProcessor();

                tlsRecordProcessor.Run().Wait();

                return 0;
            });

            commandLineApplication.Execute(args);
        }
    }
}
