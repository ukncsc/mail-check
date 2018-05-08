using System;
using System.IO;

namespace Dmarc.AggregateReport.Parser.Lambda
{
    internal class CommandLineArgs
    {
        public CommandLineArgs(string directory, string xmlDirectory, string csvFile, string sqlFile)
        {
            Directory = new DirectoryInfo(directory ?? AppContext.BaseDirectory);
            XmlDirectory = xmlDirectory != null ? new DirectoryInfo(xmlDirectory) : null;
            CsvFile = csvFile != null ? new FileInfo(csvFile) : null;
            SqlFile = sqlFile != null ? new FileInfo(sqlFile) : null;
        }

        public DirectoryInfo Directory { get; }
        public DirectoryInfo XmlDirectory { get; }
        public FileInfo CsvFile { get; }
        public FileInfo SqlFile { get; }
    }
}