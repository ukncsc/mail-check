using System;
using Dmarc.DnsRecord.Evaluator.Dmarc.Factory;
using Dmarc.DnsRecord.Evaluator.Seeding;
using Dmarc.DnsRecord.Evaluator.Seeding.Config;
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


            commandLineApplication.Command("DMARC-SEEDER", command =>
            {
                command.Description = "DMARC Record Evaluator data seeder.";

                var connectionString = command.Option("-c | --connectionString","connection string to database to read records from.", CommandOptionType.SingleValue);
                var sqsQueueUrl = command.Option("-s | --sqsQueueUrl", "sqs queue to write message to.", CommandOptionType.SingleValue);
                
                command.HelpOption("-? | -h | --help");

                command.OnExecute(() =>
                {
                    Console.WriteLine("Seeding DMARC Record Evaluator data");

                    ISeedingConfig config = new SeedingConfig(connectionString.Value(), sqsQueueUrl.Value());

                    ISeeder seeder = DmarcSeederFactory.Create(config);
                    seeder.SeedData().Wait();

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


            commandLineApplication.Command("SPF-SEEDER", command =>
            {
                command.Description = "SPF Record Evaluator data seeder.";

                var connectionString = command.Option("-c | --connectionString", "connection string to database to read records from.", CommandOptionType.SingleValue);
                var sqsQueueUrl = command.Option("-s | --sqsQueueUrl", "sqs queue to write message to.", CommandOptionType.SingleValue);

                command.HelpOption("-? | -h | --help");

                command.OnExecute(() =>
                {
                    Console.WriteLine("Seeding SPF Record Evaluator data");

                    ISeedingConfig config = new SeedingConfig(connectionString.Value(), sqsQueueUrl.Value());

                    ISeeder seeder = SpfSeederFactory.Create(config);
                    seeder.SeedData().Wait();

                    return 0;
                });
            }, false);

            commandLineApplication.Execute(args);

        }
    }
}
