using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Parsers;

namespace Dmarc.AggregateReport.Parser.App.Parsers
{
    internal interface IFileEmailMessageProcessor
    {
        void ProcessEmailMessages(DirectoryInfo directoryInfo);
    }

    internal class AggregateReportFileEmailMessageProcessor : IFileEmailMessageProcessor, IDisposable
    {
        private readonly IAggregateReportParser _aggregateReportParser;

        public AggregateReportFileEmailMessageProcessor(IAggregateReportParser aggregateReportParser)
        {
            _aggregateReportParser = aggregateReportParser;
        }

        public void ProcessEmailMessages(DirectoryInfo directoryInfo)
        {
            List<EmailMessageInfo> emailMessages = directoryInfo.GetFiles().Select(_ => new EmailMessageInfo(new EmailMetadata(_.FullName, Path.GetFileNameWithoutExtension(_.Name),  _.Length / 1024), File.Open(_.FullName, FileMode.Open))).ToList();

            emailMessages.ForEach(_aggregateReportParser.Parse);
        }

        public void Dispose()
        {
            (_aggregateReportParser as IDisposable)?.Dispose();
        }
    }
}
