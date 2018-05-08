using System;
using System.Collections.Generic;
using System.IO;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Persistence.Single;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation;

namespace Dmarc.AggregateReport.Parser.Lambda.Persistence.FileSystem
{
    public class CsvDenormalisedRecordPersistor : IDenormalisedRecordPersistor, IDisposable
    {
        private readonly FileInfo _location;
        private readonly ICsvDenormalisedRecordSerialiser _csvDenormalisedRecordSerialiser;
        private FileStream _fileStream;
        private StreamWriter _streamWriter;
        private bool _inited;

        public CsvDenormalisedRecordPersistor(FileInfo location, ICsvDenormalisedRecordSerialiser csvDenormalisedRecordSerialiser)
        {
            _location = location;
            _csvDenormalisedRecordSerialiser = csvDenormalisedRecordSerialiser;
        }

        private void CreateDirectoryAndRemoveOldFiles()
        {
            if (!_inited)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(_location.DirectoryName);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                if (_location.Exists)
                {
                    _location.Delete();
                }

                _fileStream = new FileStream(_location.FullName, FileMode.Append);
                _streamWriter = new StreamWriter(_fileStream);
                _inited = true;
            }
        }

        public void Persist(IEnumerable<DenormalisedRecord> denormalisedRecords)
        {
            CreateDirectoryAndRemoveOldFiles();
            foreach (var denormalisedRecord in denormalisedRecords)
            {
                _streamWriter.WriteLine(_csvDenormalisedRecordSerialiser.Serialise(denormalisedRecord));
            }
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
            _fileStream?.Dispose();
        }
    }
}
