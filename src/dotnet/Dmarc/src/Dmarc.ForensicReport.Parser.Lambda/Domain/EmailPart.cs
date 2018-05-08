using System.Linq;

namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public abstract class EmailPart
    {
        protected EmailPart(string contentType, int depth, Disposition disposition)
        {
            ContentType = contentType;
            Depth = depth;
            Disposition = disposition;
        }

        public string ContentType { get; }
        public int Depth { get; }
        public Disposition Disposition { get; }

        public override string ToString()
        {
            return $"{string.Join(string.Empty, Enumerable.Range(0, Depth).Select(_ => "\t"))}{nameof(ContentType)}: {ContentType} {(Disposition == null ? string.Empty : Disposition.IsAttachment ? "-> Attachment" : string.Empty)}";
        }
    }
}