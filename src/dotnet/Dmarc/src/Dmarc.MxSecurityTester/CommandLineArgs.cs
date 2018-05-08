using System.Collections.Generic;

namespace Dmarc.MxSecurityTester
{
    public class CommandLineArgs
    {
        public CommandLineArgs(List<string> debug)
        {
            Debug = debug;
        }

        public List<string> Debug { get; }
    }
}