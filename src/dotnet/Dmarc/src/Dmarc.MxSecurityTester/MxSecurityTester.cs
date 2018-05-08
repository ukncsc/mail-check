using Dmarc.MxSecurityTester.Console;
using Dmarc.MxSecurityTester.Factory;
using Dmarc.MxSecurityTester.MxTester;
using Microsoft.Extensions.CommandLineUtils;

namespace Dmarc.MxSecurityTester
{
    public class MxSecurityTester
    {
        public static void Main(string[] args)
        {
            CommandLineApplication commandLineApplication = new CommandLineApplication(false);
            commandLineApplication.Name = "MxSecurityTester";

            commandLineApplication.Command("debug", command =>
                {
                    command.Description = "Debug tls testing for hosts.";

                    CommandArgument hosts = command.Argument("[hosts]", "list of hosts space separated hosts to test", true);
                    command.HelpOption("-? | -h | --help");

                    command.OnExecute(() =>
                    {
                        IMxSecurityTesterDebugApp mxSecurityTesterDebugApp =
                            MxSecurityTesterAppFactory.CreateMxSecurityTesterDebugApp();

                        mxSecurityTesterDebugApp.Run(hosts.Values).Wait();

                        return 0;
                    });
                }, 
            false);

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                IMxSecurityTesterProcessorRunner mxSecurityTesterProcessorRunner =
                    MxSecurityTesterFactory.CreateMxSecurityTesterProcessorRunner();

                mxSecurityTesterProcessorRunner.Run().Wait();

                return 0;
            });

            commandLineApplication.Execute(args);
        }
    }
}
