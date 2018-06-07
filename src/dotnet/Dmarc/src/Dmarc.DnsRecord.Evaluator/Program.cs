using System;
using Dmarc.DnsRecord.Evaluator.Dmarc.Factory;
using Dmarc.DnsRecord.Evaluator.Spf.Factory;
using Microsoft.Extensions.CommandLineUtils;

namespace Dmarc.DnsRecord.Evaluator
{
    /*
     * The Mail Check web application parses responses from this API as GitHub Flavored Markdown: https://github.github.com/gfm/
     */
    public class Program
    {
        public static void Main(string[] args)
        {
            CommandLineApplication commandLineApplication = new CommandLineApplication();
            commandLineApplication.Name = "DnsRecordEvaluator";

            commandLineApplication.Command("DMARC", command =>
            {
                command.Description = "DMARC Record Evaluator";

                command.OnExecute(() =>
                {
                    Console.WriteLine("Running in DMARC Mode.");

                    IDmarcRecordProcessor dmarcRecordProcessor = DmarcRecordProcessorFactory.Create();
                    dmarcRecordProcessor.Run().Wait();

                    return 0;
                });
            }, false);

            commandLineApplication.Command("SPF", command =>
            {
                command.Description = "SPF Record Evaluator";

                command.OnExecute(() =>
                {
                    Console.WriteLine("Running in SPF Mode.");

                    ISpfRecordProcessor spfRecordProcessor = SpfRecordProcessorFactory.Create();
                    spfRecordProcessor.Run().Wait();

                    return 0;
                });
            }, false);

            commandLineApplication.Execute(args);
        }
    }
}
