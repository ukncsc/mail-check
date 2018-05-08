using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Linq;
using Dmarc.Common.Logging;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Report.Parser;
using Dmarc.Common.Report.Persistance;

namespace Dmarc.Common.Report.File
{
    public interface IFileEmailMessageProcessor
    {
        void ProcessEmailMessages(DirectoryInfo directoryInfo);
    }

    public class FileEmailMessageProcessor<T> : IFileEmailMessageProcessor, IDisposable 
        where T : ReportInfo
    {
        private readonly IReportParser<T> _reportParser;
        private readonly IReportPersistor<T> _reportPersistor;
        private readonly ILogger _log;

        public FileEmailMessageProcessor(IReportParser<T> reportParser,
            IReportPersistor<T> reportPersistor, 
            ILogger log)
        {
            _reportParser = reportParser;
            _reportPersistor = reportPersistor;
            _log = log;
        }

        public void ProcessEmailMessages(DirectoryInfo directoryInfo)
        {
            IEnumerable<T> reports = directoryInfo.GetFiles()
                .Select(CreateEmailMessageInfo)
                .Select(Parse)
                .Where(_ => _ != null);

            reports.ForEach(Persist);
        }

        private static EmailMessageInfo CreateEmailMessageInfo(FileInfo fileInfo)
        {
            return new EmailMessageInfo(new EmailMetadata(fileInfo.FullName, Path.GetFileNameWithoutExtension(fileInfo.Name), 
                fileInfo.Length / 1024), System.IO.File.Open(fileInfo.FullName, FileMode.Open));
        }

        private T Parse(EmailMessageInfo emailMessageInfo)
        {
            try
            {
                return _reportParser.Parse(emailMessageInfo);
            }
            catch (Exception e)
            {
                _log.Error($"Failed to process {emailMessageInfo.EmailMetadata.Filename} with error {e.Message} {System.Environment.NewLine} {e.StackTrace}");
                return null;
            }
        }

        private void Persist(T reportInfo)
        {
            try
            {
                _reportPersistor.Persist(reportInfo);
            }
            catch (Exception e)
            {
                _log.Error(
                    $"Failed to persist {reportInfo.EmailMetadata.Filename} with error {e.Message} {System.Environment.NewLine} {e.StackTrace}");
                throw;
            }
        }

        public void Dispose()
        {
            (_reportParser as IDisposable)?.Dispose();
            (_reportPersistor as IDisposable)?.Dispose();
        }
    }
}