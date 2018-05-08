using System;
using System.IO;

namespace Dmarc.ForensicReport.Parser.Lambda.Console
{
    internal class CommandLineArgs
    {
        public CommandLineArgs(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentException($"{nameof(directory)} should not be null");
            }

            Directory = new DirectoryInfo(directory);
        }

        public DirectoryInfo Directory { get; }
    }
}
