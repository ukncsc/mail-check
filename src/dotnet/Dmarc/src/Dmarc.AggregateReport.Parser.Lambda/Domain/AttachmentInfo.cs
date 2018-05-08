using System;
using System.IO;

namespace Dmarc.AggregateReport.Parser.Lambda.Domain
{
    public class AttachmentInfo : IDisposable
    {
        public static AttachmentInfo EmptyAttachmentInfo = new AttachmentInfo(AttachmentMetadata.EmptyAttachmentMetadata, Stream.Null);

        private Stream _stream;

        public AttachmentInfo(AttachmentMetadata attachmentMetadata, Stream stream)
        {
            _stream = stream;
            AttachmentMetadata = attachmentMetadata;
        }

        public AttachmentMetadata AttachmentMetadata { get; }


        public Stream GetStream()
        {
            MemoryStream memoryStream = new MemoryStream();
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        protected bool Equals(AttachmentInfo other)
        {
            return Equals(AttachmentMetadata, other.AttachmentMetadata);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AttachmentInfo)obj);
        }

        public override int GetHashCode()
        {
            return (AttachmentMetadata != null ? AttachmentMetadata.GetHashCode() : 0);
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _stream = null;
        }
    }
}