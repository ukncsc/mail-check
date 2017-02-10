using System.Collections.Generic;
using System.IO;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Persistence.Single;

namespace Dmarc.AggregateReport.Parser.Common.Persistence.FileSystem
{
    public class XmlAttachmentPersitor : IAttachmentPersitor
    {
        private readonly DirectoryInfo _location;
        private bool _inited;

        public XmlAttachmentPersitor(DirectoryInfo location)
        {
            _location = location;
            CreateDirectory();   
        }

        public void Persist(IEnumerable<AttachmentInfo> attachments)
        {
            foreach (var attachment in attachments)
            {
                using (var fileStream = File.Create($"{_location.FullName}/{attachment.AttachmentMetadata.Filename}.xml"))
                {
                    using (var stream = attachment.GetStream())
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
        }

        private void CreateDirectory()
        {
            if (!_inited)
            {
                if (!_location.Exists)
                {
                    _location.Create();
                }
                _inited = true;
            }
        }

        public void Dispose(){}
    }
}
