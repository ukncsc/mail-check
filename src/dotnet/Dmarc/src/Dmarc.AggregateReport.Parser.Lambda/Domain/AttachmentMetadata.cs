namespace Dmarc.AggregateReport.Parser.Lambda.Domain
{
    public class AttachmentMetadata
    {
        public static AttachmentMetadata EmptyAttachmentMetadata = new AttachmentMetadata(string.Empty);

        public AttachmentMetadata(string filename)
        {
            Filename = filename;
        }

        public string Filename { get; }

        protected bool Equals(AttachmentMetadata other)
        {
            return string.Equals(Filename, other.Filename);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AttachmentMetadata)obj);
        }

        public override int GetHashCode()
        {
            return (Filename != null ? Filename.GetHashCode() : 0);
        }
    }
}